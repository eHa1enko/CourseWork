import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '../core/services/auth.service';
import { PlayerService } from '../core/services/player.service';
import { SongsService } from '../core/services/songs.service';
import { SongDto } from '../core/models/song.dto';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe],
  templateUrl: './layout.html',
  styleUrl: './layout.css'
})
export class Layout {
  readonly auth = inject(AuthService);
  readonly player = inject(PlayerService);
  private readonly songsService = inject(SongsService);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  hoverFraction: number | null = null;
  private lastVolume = 1;

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
