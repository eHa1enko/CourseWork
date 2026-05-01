import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { SongsService } from '../../core/services/songs.service';
import { PlayerService } from '../../core/services/player.service';
import { SongDto } from '../../core/models/song.dto';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-liked-songs',
  imports: [],
  templateUrl: './liked-songs.html',
  styleUrl: './liked-songs.css'
})
export class LikedSongs implements OnInit, OnDestroy {
  private readonly songsService = inject(SongsService);
  readonly player = inject(PlayerService);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  songs = signal<SongDto[]>([]);
  loading = signal(true);

  private likeSub?: Subscription;

  ngOnInit() {
    this.songsService.getLikedSongs().subscribe({
      next: songs => {
        this.songs.set(songs);
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); }
    });
    this.likeSub = this.songsService.likeChanged$.subscribe(({ songId, isLiked }) => {
      if (isLiked) return;
      this.songs.update(list => list.filter(s => s.id !== songId));
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

  unlike(song: SongDto, event: Event) {
    event.stopPropagation();
    this.songsService.unlikeSong(song.id).subscribe({
      next: () => {
        this.songs.update(list => list.filter(s => s.id !== song.id));
        this.songsService.notifyLikeChanged(song.id, false);
        if (this.player.currentSong?.id === song.id) {
          this.player.updateCurrentSongLike(false);
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
