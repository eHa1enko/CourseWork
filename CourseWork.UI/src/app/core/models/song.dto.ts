export interface SongDto {
  id: number;
  title: string;
  duration: number;
  coverUrl: string;
  artistId: number;
  artistName: string;
  isLiked: boolean;
}
