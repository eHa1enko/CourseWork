import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { ArtistsService } from '../../core/services/artists.service';
import { SongsService } from '../../core/services/songs.service';
import { PlayerService } from '../../core/services/player.service';
import { ArtistDto } from '../../core/models/artist.dto';
import { SongDto } from '../../core/models/song.dto';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-artist-detail',
  imports: [],
  templateUrl: './artist-detail.html',
  styleUrl: './artist-detail.css'
})
export class ArtistDetail implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly artistsService = inject(ArtistsService);
  private readonly songsService = inject(SongsService);
  readonly player = inject(PlayerService);
  readonly location = inject(Location);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  artist = signal<ArtistDto | null>(null);
  songs = signal<SongDto[]>([]);
  loading = signal(true);

  private likeSub?: Subscription;

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.artistsService.getById(id).subscribe(a => { this.artist.set(a); });
    this.artistsService.getSongs(id).subscribe({
      next: songs => {
        this.songs.set(songs);
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); }
    });
    this.likeSub = this.songsService.likeChanged$.subscribe(({ songId, isLiked }) => {
      this.songs.update(list =>
        list.map(s => s.id === songId ? { ...s, isLiked } : s)
      );
    });
  }

  ngOnDestroy() {
    this.likeSub?.unsubscribe();
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
