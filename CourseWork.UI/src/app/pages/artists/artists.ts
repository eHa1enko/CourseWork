import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ArtistsService } from '../../core/services/artists.service';
import { ArtistDto } from '../../core/models/artist.dto';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-artists',
  imports: [],
  templateUrl: './artists.html',
  styleUrl: './artists.css'
})
export class Artists implements OnInit {
  private readonly artistsService = inject(ArtistsService);
  private readonly router = inject(Router);
  readonly apiBase = environment.apiUrl.replace('/api', '');

  artists = signal<ArtistDto[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.artistsService.getAll().subscribe({
      next: artists => {
        this.artists.set(artists);
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); }
    });
  }

  openArtist(id: number) {
    this.router.navigate(['/artists', id]);
  }
}
