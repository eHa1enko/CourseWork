import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { SongDto } from '../models/song.dto';

@Injectable({ providedIn: 'root' })
export class SongsService {
  private readonly api = inject(ApiService);

  getAll() {
    return this.api.get<SongDto[]>('songs');
  }
}
