import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SongDto } from '../models/song.dto';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PlayerService {
  private readonly audio = new Audio();
  private readonly streamBase = environment.apiUrl;

  private queue: SongDto[] = [];
  private shuffledQueue: SongDto[] = [];
  private queueIndex = -1;

  private readonly _currentSong$ = new BehaviorSubject<SongDto | null>(null);
  private readonly _isPlaying$ = new BehaviorSubject<boolean>(false);
  private readonly _currentTime$ = new BehaviorSubject<number>(0);
  private readonly _progress$ = new BehaviorSubject<number>(0);
  private readonly _shuffle$ = new BehaviorSubject<boolean>(false);

  readonly currentSong$ = this._currentSong$.asObservable();

  get currentSong(): SongDto | null {
    return this._currentSong$.value;
  }
  readonly isPlaying$ = this._isPlaying$.asObservable();
  readonly currentTime$ = this._currentTime$.asObservable();
  readonly progress$ = this._progress$.asObservable();
  readonly shuffle$ = this._shuffle$.asObservable();

  constructor() {
    this.audio.addEventListener('timeupdate', () => {
      if (this.audio.duration) {
        this._currentTime$.next(this.audio.currentTime);
        this._progress$.next(this.audio.currentTime / this.audio.duration);
      }
    });

    this.audio.addEventListener('play', () => this._isPlaying$.next(true));
    this.audio.addEventListener('pause', () => this._isPlaying$.next(false));
    this.audio.addEventListener('ended', () => {
      this._progress$.next(0);
      this._currentTime$.next(0);
      this.next();
    });
  }

  setQueue(songs: SongDto[], startIndex = 0) {
    this.queue = songs;
    this.shuffledQueue = this.buildShuffled(songs, startIndex);
    this.queueIndex = startIndex;
    this.loadAndPlay(this.activeQueue[startIndex]);
  }

  play(song: SongDto) {
    const idx = this.queue.findIndex(s => s.id === song.id);
    if (idx !== -1) {
      this.queueIndex = idx;
      this.loadAndPlay(song);
    } else {
      this.queue = [song];
      this.shuffledQueue = [song];
      this.queueIndex = 0;
      this.loadAndPlay(song);
    }
  }

  toggle() {
    if (this.audio.paused) {
      this.audio.play();
    } else {
      this.audio.pause();
    }
  }

  next() {
    const q = this.activeQueue;
    if (q.length === 0) return;
    this.queueIndex = (this.queueIndex + 1) % q.length;
    this.loadAndPlay(q[this.queueIndex]);
  }

  prev() {
    if (this.audio.currentTime > 3) {
      this.audio.currentTime = 0;
      return;
    }
    const q = this.activeQueue;
    if (q.length === 0) return;
    this.queueIndex = (this.queueIndex - 1 + q.length) % q.length;
    this.loadAndPlay(q[this.queueIndex]);
  }

  toggleShuffle() {
    const next = !this._shuffle$.value;
    this._shuffle$.next(next);
    if (next) {
      this.shuffledQueue = this.buildShuffled(this.queue, this.queueIndex);
      this.queueIndex = 0;
    }
  }

  seek(fraction: number) {
    if (this.audio.duration) {
      this.audio.currentTime = fraction * this.audio.duration;
    }
  }

  private get activeQueue(): SongDto[] {
    return this._shuffle$.value ? this.shuffledQueue : this.queue;
  }

  private loadAndPlay(song: SongDto) {
    this._currentSong$.next(song);
    this.audio.src = `${this.streamBase}/songs/${song.id}/stream`;
    this.audio.play();
  }

  private buildShuffled(songs: SongDto[], currentIndex: number): SongDto[] {
    const current = songs[currentIndex];
    const rest = songs.filter((_, i) => i !== currentIndex);
    for (let i = rest.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [rest[i], rest[j]] = [rest[j], rest[i]];
    }
    return current ? [current, ...rest] : [...rest];
  }
}
