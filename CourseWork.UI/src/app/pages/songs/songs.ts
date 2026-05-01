import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { SongsService } from '../../core/services/songs.service';
import { PlayerService } from '../../core/services/player.service';
import { SongDto } from '../../core/models/song.dto';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-songs',
  imports: [],
  templateUrl: './songs.html',
  styleUrl: './songs.css'
})
export class Songs implements OnInit, OnDestroy {
  private readonly songsService = inject(SongsService);
  readonly player = inject(PlayerService);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  songs = signal<SongDto[]>([]);
  loading = signal(true);

  private likeSub?: Subscription;

  ngOnInit() {
    this.songsService.getAll().subscribe({
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
