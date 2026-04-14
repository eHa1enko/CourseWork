import { Component, inject, OnInit, signal } from '@angular/core';
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
export class ArtistDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly artistsService = inject(ArtistsService);
  private readonly songsService = inject(SongsService);
  readonly player = inject(PlayerService);
  readonly location = inject(Location);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  artist = signal<ArtistDto | null>(null);
  songs = signal<SongDto[]>([]);
  loading = signal(true);

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
    this.songs.update(list =>
      list.map(s => s.id === song.id ? { ...s, isLiked: !wasLiked } : s)
    );
    const request = wasLiked
      ? this.songsService.unlikeSong(song.id)
      : this.songsService.likeSong(song.id);
    request.subscribe({
      error: () => {
        this.songs.update(list =>
          list.map(s => s.id === song.id ? { ...s, isLiked: wasLiked } : s)
        );
      }
    });
    if (this.player.currentSong?.id === song.id) {
      this.player.updateCurrentSongLike(!wasLiked);
    }
  }

  formatDuration(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
