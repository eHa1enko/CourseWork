export interface SongDto {
  id: number;
  title: string;
  duration: number;
  fileUrl: string;
  coverUrl: string;
  artistId: number;
  artistName: string;
  isLiked: boolean;
}
