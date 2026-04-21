import { Component, inject, signal, OnDestroy } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '../core/services/auth.service';
import { PlayerService } from '../core/services/player.service';
import { SongsService } from '../core/services/songs.service';
import { SearchService } from '../core/services/search.service';
import { SongDto } from '../core/models/song.dto';
import { ArtistDto } from '../core/models/artist.dto';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe],
  templateUrl: './layout.html',
  styleUrl: './layout.css'
})
export class Layout implements OnDestroy {
  readonly auth = inject(AuthService);
  readonly player = inject(PlayerService);
  private readonly songsService = inject(SongsService);
  private readonly searchService = inject(SearchService);
  private readonly router = inject(Router);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  hoverFraction: number | null = null;
  private lastVolume = 1;

  searchQuery = signal('');
  dropdownArtists = signal<ArtistDto[]>([]);
  dropdownSongs = signal<SongDto[]>([]);
  showDropdown = signal(false);
  searchLoading = signal(false);

  private debounceTimer: ReturnType<typeof setTimeout> | null = null;
  private closeTimer: ReturnType<typeof setTimeout> | null = null;

  ngOnDestroy() {
    if (this.debounceTimer) clearTimeout(this.debounceTimer);
    if (this.closeTimer) clearTimeout(this.closeTimer);
  }

  onSearchInput(event: Event) {
    const query = (event.target as HTMLInputElement).value;
    this.searchQuery.set(query);

    if (this.debounceTimer) clearTimeout(this.debounceTimer);

    if (!query.trim()) {
      this.showDropdown.set(false);
      this.dropdownArtists.set([]);
      this.dropdownSongs.set([]);
      return;
    }

    this.searchLoading.set(true);
    this.showDropdown.set(true);

    this.debounceTimer = setTimeout(() => {
      this.searchService.search(query).subscribe({
        next: result => {
          this.dropdownArtists.set(result.artists.slice(0, 3));
          this.dropdownSongs.set(result.songs.slice(0, 4));
          this.searchLoading.set(false);
        },
        error: () => { this.searchLoading.set(false); }
      });
    }, 300);
  }

  onSearchKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      const query = this.searchQuery().trim();
      this.closeDropdown();
      if (query) {
        this.router.navigate(['/search'], { queryParams: { q: query } });
      }
    } else if (event.key === 'Escape') {
      this.closeDropdown();
    }
  }

  onSearchFocus() {
    if (this.closeTimer) clearTimeout(this.closeTimer);
    if (this.searchQuery().trim() && (this.dropdownArtists().length > 0 || this.dropdownSongs().length > 0)) {
      this.showDropdown.set(true);
    }
  }

  onSearchBlur() {
    this.closeTimer = setTimeout(() => this.showDropdown.set(false), 150);
  }

  closeDropdown() {
    this.showDropdown.set(false);
  }

  goToSearchPage() {
    const query = this.searchQuery().trim();
    this.closeDropdown();
    if (query) {
      this.router.navigate(['/search'], { queryParams: { q: query } });
    }
  }

  openArtistFromDropdown(artist: ArtistDto) {
    this.closeDropdown();
    this.router.navigate(['/artists', artist.id]);
  }

  playSongFromDropdown(song: SongDto) {
    this.closeDropdown();
    const queue = this.dropdownSongs();
    this.player.setQueue(queue, queue.indexOf(song));
  }

  toggleLike(song: SongDto) {
    const wasLiked = song.isLiked;
    this.player.updateCurrentSongLike(!wasLiked);
    const request = wasLiked
      ? this.songsService.unlikeSong(song.id)
      : this.songsService.likeSong(song.id);
    request.subscribe({
      error: () => this.player.updateCurrentSongLike(wasLiked)
    });
  }

  onSeek(event: MouseEvent) {
    const bar = event.currentTarget as HTMLElement;
    this.player.seek(event.offsetX / bar.offsetWidth);
  }

  onBarHover(event: MouseEvent) {
    const bar = event.currentTarget as HTMLElement;
    this.hoverFraction = Math.min(1, Math.max(0, event.offsetX / bar.offsetWidth));
  }

  onVolumeChange(event: Event) {
    const value = parseFloat((event.target as HTMLInputElement).value);
    this.lastVolume = value || this.lastVolume;
    this.player.setVolume(value);
  }

  logout() {
    this.player.stop();
    this.auth.logout();
  }

  toggleMute() {
    if (this.player.volume > 0) {
      this.lastVolume = this.player.volume;
      this.player.setVolume(0);
    } else {
      this.player.setVolume(this.lastVolume);
    }
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = Math.floor(seconds % 60);
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
