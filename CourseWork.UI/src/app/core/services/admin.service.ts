import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ArtistDto } from '../models/artist.dto';
import { SongDto } from '../models/song.dto';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  createArtist(name: string, image?: File) {
    const form = new FormData();
    form.append('name', name);
    if (image) form.append('image', image);
    return this.http.post<ArtistDto>(`${this.base}/admin/artists`, form);
  }

  deleteArtist(id: number) {
    return this.http.delete(`${this.base}/admin/artists/${id}`);
  }

  createSong(title: string, artistId: number, audio: File, cover?: File) {
    const form = new FormData();
    form.append('title', title);
    form.append('artistId', artistId.toString());
    form.append('audio', audio);
    if (cover) form.append('cover', cover);
    return this.http.post<SongDto>(`${this.base}/admin/songs`, form);
  }

  deleteSong(id: number) {
    return this.http.delete(`${this.base}/admin/songs/${id}`);
  }
}
