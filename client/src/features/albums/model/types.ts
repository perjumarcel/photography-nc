export type LoadStatus = 'idle' | 'loading' | 'succeeded' | 'failed';

export interface AlbumDto {
  id: string;
  title: string;
  slug: string;
  description?: string | null;
  eventDate?: string | null;
  client?: string | null;
  location?: string | null;
  seoTitle?: string | null;
  seoDescription?: string | null;
  coverAltText?: string | null;
  showInPortfolio: boolean;
  showInStories: boolean;
  showInHome: boolean;
  categoryId: number;
  imageCount: number;
  coverImageId?: string | null;
  coverPublicUrl?: string | null;
  coverWidth?: number | null;
  coverHeight?: number | null;
  coverVariants?: ImageVariantsDto | null;
}

export interface ImageVariantsDto {
  placeholder: string;
  thumbnail: string;
  card: string;
  hero: string;
  full: string;
}

export interface ImageDto {
  id: string;
  albumId: string;
  originalName: string;
  storageKey: string;
  publicUrl: string;
  variants: ImageVariantsDto;
  width: number;
  height: number;
  orientation: number;
  imageType: number;
  sizeBytes: number;
}

export interface AlbumDetailsDto extends AlbumDto {
  images: ImageDto[];
}
