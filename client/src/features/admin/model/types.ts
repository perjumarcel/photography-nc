import type { LoadStatus, AlbumDto, AlbumDetailsDto } from '@/features/albums/model/types';
import type { CategoryDto } from '@/features/categories/model/types';

export interface CreateAlbumDto {
  title: string;
  categoryId: number;
  slug?: string | null;
  description?: string | null;
  eventDate?: string | null;
  client?: string | null;
  location?: string | null;
  seoTitle?: string | null;
  seoDescription?: string | null;
  coverAltText?: string | null;
  showInPortfolio?: boolean;
  showInStories?: boolean;
  showInHome?: boolean;
}

export type UpdateAlbumDto = Required<Pick<CreateAlbumDto,
  'title' | 'categoryId' | 'showInPortfolio' | 'showInStories' | 'showInHome'>> &
  Pick<CreateAlbumDto, 'slug' | 'description' | 'eventDate' | 'client' | 'location' | 'seoTitle' | 'seoDescription' | 'coverAltText'>;

export interface CreateCategoryDto {
  name: string;
  slug?: string | null;
  displayOrder: number;
  showAsFilter?: boolean;
}

export type UpdateCategoryDto = Required<CreateCategoryDto>;

export interface AdminState {
  /** Full album list shown in the admin table (includes drafts/hidden). */
  albums: AlbumDto[];
  albumsStatus: LoadStatus;
  albumsError: string | null;
  /** Full category list (admin view). */
  categories: CategoryDto[];
  categoriesStatus: LoadStatus;
  categoriesError: string | null;
  /** Currently-edited album, with its full image list. */
  current: AlbumDetailsDto | null;
  currentStatus: LoadStatus;
  currentError: string | null;
  /** Status of the most recent mutation (create/update/delete/upload). */
  mutationStatus: LoadStatus;
  mutationError: string | null;
}
