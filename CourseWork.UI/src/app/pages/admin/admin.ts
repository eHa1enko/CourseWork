import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ArtistsService } from '../../core/services/artists.service';
import { SongsService } from '../../core/services/songs.service';
import { ArtistDto } from '../../core/models/artist.dto';
import { SongDto } from '../../core/models/song.dto';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admin',
  imports: [FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class Admin implements OnInit {
  private readonly adminService = inject(AdminService);
  private readonly artistsService = inject(ArtistsService);
  private readonly songsService = inject(SongsService);

  readonly apiBase = environment.apiUrl.replace('/api', '');

  artists = signal<ArtistDto[]>([]);
  songs = signal<SongDto[]>([]);

  // Artist form
  artistName = '';
  artistImageFile: File | null = null;
  artistLoading = false;
  artistError = '';

  // Song form
  songTitle = '';
  songArtistId: number | null = null;
  songAudioFile: File | null = null;
  songCoverFile: File | null = null;
  songLoading = false;
  songError = '';

  ngOnInit() {
    this.loadArtists();
    this.loadSongs();
  }

  private loadArtists() {
    this.artistsService.getAll().subscribe(a => this.artists.set(a));
  }

  private loadSongs() {
    this.songsService.getAll().subscribe(s => this.songs.set(s));
  }

  onArtistImage(event: Event) {
    this.artistImageFile = (event.target as HTMLInputElement).files?.[0] ?? null;
  }

  onSongAudio(event: Event) {
    this.songAudioFile = (event.target as HTMLInputElement).files?.[0] ?? null;
  }

  onSongCover(event: Event) {
    this.songCoverFile = (event.target as HTMLInputElement).files?.[0] ?? null;
  }

  submitArtist() {
    if (!this.artistName.trim()) return;
    this.artistLoading = true;
    this.artistError = '';

    this.adminService.createArtist(this.artistName, this.artistImageFile ?? undefined).subscribe({
      next: (artist) => {
        this.artists.update(list => [...list, artist]);
        this.artistName = '';
        this.artistImageFile = null;
        this.artistLoading = false;
      },
      error: () => {
        this.artistError = 'Помилка при створенні артиста.';
        this.artistLoading = false;
      }
    });
  }

  deleteArtist(artist: ArtistDto) {
    if (!confirm(`Видалити артиста "${artist.name}" і всі його треки?`)) return;

    this.adminService.deleteArtist(artist.id).subscribe({
      next: () => {
        this.artists.update(list => list.filter(a => a.id !== artist.id));
        this.songs.update(list => list.filter(s => s.artistId !== artist.id));
      }
    });
  }

  submitSong() {
    if (!this.songTitle.trim() || !this.songArtistId || !this.songAudioFile) return;
    this.songLoading = true;
    this.songError = '';

    this.adminService.createSong(
      this.songTitle,
      this.songArtistId,
      this.songAudioFile,
      this.songCoverFile ?? undefined
    ).subscribe({
      next: (song) => {
        this.songs.update(list => [...list, song]);
        this.songTitle = '';
        this.songArtistId = null;
        this.songAudioFile = null;
        this.songCoverFile = null;
        this.songLoading = false;
      },
      error: () => {
        this.songError = 'Помилка при додаванні треку.';
        this.songLoading = false;
      }
    });
  }

  deleteSong(song: SongDto) {
    if (!confirm(`Видалити трек "${song.title}"?`)) return;

    this.adminService.deleteSong(song.id).subscribe({
      next: () => this.songs.update(list => list.filter(s => s.id !== song.id))
    });
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = Math.floor(seconds % 60);
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
