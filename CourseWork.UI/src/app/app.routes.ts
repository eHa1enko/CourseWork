import { Routes } from '@angular/router';
import { Layout } from './layout/layout';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register').then(m => m.Register)
  },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'songs', pathMatch: 'full' },
      {
        path: 'songs',
        loadComponent: () => import('./pages/songs/songs').then(m => m.Songs)
      },
      {
        path: 'artists',
        loadComponent: () => import('./pages/artists/artists').then(m => m.Artists)
      },
      {
        path: 'artists/:id',
        loadComponent: () => import('./pages/artist-detail/artist-detail').then(m => m.ArtistDetail)
      },
      {
        path: 'liked-songs',
        loadComponent: () => import('./pages/liked-songs/liked-songs').then(m => m.LikedSongs)
      }
    ]
  }
];
