import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { ArtistDto } from '../models/artist.dto';
import { SongDto } from '../models/song.dto';

@Injectable({ providedIn: 'root' })
export class ArtistsService {
  private readonly api = inject(ApiService);

  getAll() {
    return this.api.get<ArtistDto[]>('artists');
  }

  getById(id: number) {
    return this.api.get<ArtistDto>(`artists/${id}`);
  }

  getSongs(id: number) {
    return this.api.get<SongDto[]>(`artists/${id}/songs`);
  }
}
