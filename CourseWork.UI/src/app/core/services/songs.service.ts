import { Injectable, inject } from '@angular/core';
import { Subject } from 'rxjs';
import { ApiService } from './api.service';
import { SongDto } from '../models/song.dto';

@Injectable({ providedIn: 'root' })
export class SongsService {
  private readonly api = inject(ApiService);

  private readonly _likeChanged$ = new Subject<{ songId: number; isLiked: boolean }>();
  readonly likeChanged$ = this._likeChanged$.asObservable();

  getAll() {
    return this.api.get<SongDto[]>('songs');
  }

  getLikedSongs() {
    return this.api.get<SongDto[]>('liked-songs');
  }

  likeSong(songId: number) {
    return this.api.post<void>(`liked-songs/${songId}`, {});
  }

  unlikeSong(songId: number) {
    return this.api.delete<void>(`liked-songs/${songId}`);
  }

  notifyLikeChanged(songId: number, isLiked: boolean) {
    this._likeChanged$.next({ songId, isLiked });
  }
}
