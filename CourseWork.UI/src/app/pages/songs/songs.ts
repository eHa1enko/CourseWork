import { Component, inject, OnInit, signal } from '@angular/core';
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
export class Songs implements OnInit {
  private readonly songsService = inject(SongsService);
  readonly player = inject(PlayerService);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  songs = signal<SongDto[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.songsService.getAll().subscribe({
      next: songs => {
        this.songs.set(songs);
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); }
    });
  }

  playSong(song: SongDto) {
    this.player.setQueue(this.songs(), this.songs().indexOf(song));
  }

  isCurrentSong(song: SongDto): boolean {
    return this.player.currentSong?.id === song.id;
  }

  formatDuration(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
