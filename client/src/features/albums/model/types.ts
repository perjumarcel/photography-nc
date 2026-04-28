export type LoadStatus = 'idle' | 'loading' | 'succeeded' | 'failed';

export interface AlbumDto {
  id: string;
  title: string;
  description?: string | null;
  eventDate?: string | null;
  client?: string | null;
  location?: string | null;
  showInPortfolio: boolean;
  showInStories: boolean;
  showInHome: boolean;
  categoryId: number;
  imageCount: number;
  coverImageId?: string | null;
}

export interface ImageDto {
  id: string;
  albumId: string;
  originalName: string;
  storageKey: string;
  publicUrl: string;
  width: number;
  height: number;
  orientation: number;
  imageType: number;
  sizeBytes: number;
}

export interface AlbumDetailsDto extends AlbumDto {
  images: ImageDto[];
}
