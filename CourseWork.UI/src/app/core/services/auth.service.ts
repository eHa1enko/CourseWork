import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, tap } from 'rxjs';
import { ApiService } from './api.service';

export interface UserDto {
  id: number;
  username: string;
  email: string;
}

export interface AuthResultDto {
  token: string;
  user: UserDto;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  private readonly _currentUser$ = new BehaviorSubject<UserDto | null>(this.loadUser());
  readonly currentUser$ = this._currentUser$.asObservable();
  readonly isLoggedIn$ = new BehaviorSubject<boolean>(!!localStorage.getItem('token'));

  private loadUser(): UserDto | null {
    const raw = localStorage.getItem('user');
    return raw ? JSON.parse(raw) : null;
  }

  get currentUser(): UserDto | null {
    return this._currentUser$.value;
  }

  login(email: string, password: string) {
    return this.api.post<AuthResultDto>('auth/login', { email, password }).pipe(
      tap(result => this.handleAuth(result))
    );
  }

  register(username: string, email: string, password: string) {
    return this.api.post<AuthResultDto>('auth/register', { username, email, password }).pipe(
      tap(result => this.handleAuth(result))
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this._currentUser$.next(null);
    this.isLoggedIn$.next(false);
    this.router.navigate(['/login']);
  }

  private handleAuth(result: AuthResultDto) {
    localStorage.setItem('token', result.token);
    localStorage.setItem('user', JSON.stringify(result.user));
    this._currentUser$.next(result.user);
    this.isLoggedIn$.next(true);
  }
}
