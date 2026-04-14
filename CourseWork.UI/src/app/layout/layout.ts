import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '../core/services/auth.service';
import { PlayerService } from '../core/services/player.service';
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
  readonly apiBase = environment.apiUrl.replace('/api', '');

  hoverFraction: number | null = null;

  onSeek(event: MouseEvent) {
    const bar = event.currentTarget as HTMLElement;
    this.player.seek(event.offsetX / bar.offsetWidth);
  }

  onBarHover(event: MouseEvent) {
    const bar = event.currentTarget as HTMLElement;
    this.hoverFraction = Math.min(1, Math.max(0, event.offsetX / bar.offsetWidth));
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = Math.floor(seconds % 60);
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
