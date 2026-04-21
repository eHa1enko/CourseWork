import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { ArtistDto } from '../models/artist.dto';
import { SongDto } from '../models/song.dto';

export interface SearchResultDto {
  artists: ArtistDto[];
  songs: SongDto[];
}

@Injectable({ providedIn: 'root' })
export class SearchService {
  private readonly api = inject(ApiService);

  search(query: string) {
    return this.api.get<SearchResultDto>(`search?q=${encodeURIComponent(query)}`);
  }
}
