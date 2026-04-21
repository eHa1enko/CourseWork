import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SearchService, SearchResultDto } from '../../core/services/search.service';
import { PlayerService } from '../../core/services/player.service';
import { SongsService } from '../../core/services/songs.service';
import { SongDto } from '../../core/models/song.dto';
import { ArtistDto } from '../../core/models/artist.dto';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-search',
  imports: [],
  templateUrl: './search.html',
  styleUrl: './search.css'
})
export class Search implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly searchService = inject(SearchService);
  private readonly songsService = inject(SongsService);
  readonly player = inject(PlayerService);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  query = signal('');
  artists = signal<ArtistDto[]>([]);
  songs = signal<SongDto[]>([]);
  loading = signal(false);
  searched = signal(false);

  private routeSub?: Subscription;
  private likeSub?: Subscription;

  ngOnInit() {
    this.routeSub = this.route.queryParamMap.subscribe(params => {
      const q = params.get('q') ?? '';
      this.query.set(q);
      if (q.trim()) {
        this.doSearch(q);
      } else {
        this.artists.set([]);
        this.songs.set([]);
        this.searched.set(false);
      }
    });

    this.likeSub = this.songsService.likeChanged$.subscribe(({ songId, isLiked }) => {
      this.songs.update(list =>
        list.map(s => s.id === songId ? { ...s, isLiked } : s)
      );
    });
  }

  ngOnDestroy() {
    this.routeSub?.unsubscribe();
    this.likeSub?.unsubscribe();
  }

  private doSearch(q: string) {
    this.loading.set(true);
    this.searched.set(false);
    this.searchService.search(q).subscribe({
      next: result => {
        this.artists.set(result.artists);
        this.songs.set(result.songs);
        this.loading.set(false);
        this.searched.set(true);
      },
      error: () => { this.loading.set(false); this.searched.set(true); }
    });
  }

  openArtist(id: number) {
    this.router.navigate(['/artists', id]);
  }

  playSong(song: SongDto) {
    this.player.setQueue(this.songs(), this.songs().indexOf(song));
  }

  isCurrentSong(song: SongDto): boolean {
    return this.player.currentSong?.id === song.id;
  }

  toggleLike(song: SongDto, event: Event) {
    event.stopPropagation();
    const wasLiked = song.isLiked;
    const newLiked = !wasLiked;
    this.songs.update(list =>
      list.map(s => s.id === song.id ? { ...s, isLiked: newLiked } : s)
    );
    this.songsService.notifyLikeChanged(song.id, newLiked);
    if (this.player.currentSong?.id === song.id) {
      this.player.updateCurrentSongLike(newLiked);
    }
    const request = wasLiked
      ? this.songsService.unlikeSong(song.id)
      : this.songsService.likeSong(song.id);
    request.subscribe({
      error: () => {
        this.songs.update(list =>
          list.map(s => s.id === song.id ? { ...s, isLiked: wasLiked } : s)
        );
        this.songsService.notifyLikeChanged(song.id, wasLiked);
        if (this.player.currentSong?.id === song.id) {
          this.player.updateCurrentSongLike(wasLiked);
        }
      }
    });
  }

  formatDuration(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
