import { describe, expect, it } from 'vitest';
import adminReducer, { resetMutation, clearCurrent } from './adminSlice';
import {
  createAdminAlbum, deleteAdminAlbum, fetchAdminAlbums, fetchAdminCategories,
  setAlbumCover, deleteAlbumImage,
} from '../api/thunks';
import type { AlbumDto, ImageDto, AlbumDetailsDto } from '@/features/albums/model/types';
import type { CategoryDto } from '@/features/categories/model/types';

const album: AlbumDto = {
  id: 'a1', title: 'Album 1', showInPortfolio: true, showInStories: false, showInHome: false,
  categoryId: 1, imageCount: 0, coverPublicUrl: '/cover.jpg', coverWidth: 100, coverHeight: 50,
  coverVariants: {
    placeholder: '/cover.jpg?width=40',
    thumbnail: '/cover.jpg?width=240',
    card: '/cover.jpg?width=640',
    hero: '/cover.jpg?width=1600',
    full: '/cover.jpg',
  },
};
const album2: AlbumDto = { ...album, id: 'a2', title: 'Album 2' };
const category: CategoryDto = { id: 1, name: 'Wedding', slug: 'wedding', displayOrder: 10, showAsFilter: true };
const image: ImageDto = {
  id: 'img1', albumId: 'a1', originalName: 'p.jpg', storageKey: 'k', publicUrl: '/k',
  variants: {
    placeholder: '/k?width=40',
    thumbnail: '/k?width=240',
    card: '/k?width=640',
    hero: '/k?width=1600',
    full: '/k',
  },
  width: 100, height: 50, orientation: 0, imageType: 0, sizeBytes: 1234,
};
const albumDetails: AlbumDetailsDto = { ...album, images: [image] };

describe('adminSlice', () => {
  it('starts idle', () => {
    const state = adminReducer(undefined, { type: '@@init' });
    expect(state.albumsStatus).toBe('idle');
    expect(state.mutationStatus).toBe('idle');
  });

  it('hydrates albums on fetch fulfilled', () => {
    const state = adminReducer(undefined, { type: fetchAdminAlbums.fulfilled.type, payload: [album, album2] });
    expect(state.albums).toHaveLength(2);
    expect(state.albumsStatus).toBe('succeeded');
  });

  it('removes the deleted album from the list', () => {
    const seeded = adminReducer(undefined, { type: fetchAdminAlbums.fulfilled.type, payload: [album, album2] });
    const state = adminReducer(seeded, { type: deleteAdminAlbum.fulfilled.type, payload: 'a1' });
    expect(state.albums.map((a) => a.id)).toEqual(['a2']);
  });

  it('hydrates categories on fetch fulfilled', () => {
    const state = adminReducer(undefined, { type: fetchAdminCategories.fulfilled.type, payload: [category] });
    expect(state.categories).toHaveLength(1);
  });

  it('marks mutation pending/fulfilled/rejected via matcher', () => {
    const pending = adminReducer(undefined, { type: createAdminAlbum.pending.type });
    expect(pending.mutationStatus).toBe('loading');

    const fulfilled = adminReducer(pending, { type: createAdminAlbum.fulfilled.type, payload: { id: 'x' } });
    expect(fulfilled.mutationStatus).toBe('succeeded');

    const rejected = adminReducer(fulfilled, { type: createAdminAlbum.rejected.type, payload: 'boom' });
    expect(rejected.mutationStatus).toBe('failed');
    expect(rejected.mutationError).toBe('boom');
  });

  it('resetMutation clears mutation status', () => {
    const seeded = adminReducer(undefined, { type: createAdminAlbum.rejected.type, payload: 'boom' });
    const state = adminReducer(seeded, resetMutation());
    expect(state.mutationStatus).toBe('idle');
    expect(state.mutationError).toBeNull();
  });

  it('removes image from current album on delete', () => {
    const seeded = adminReducer(undefined, { type: 'admin/albums/fetchOne/fulfilled', payload: albumDetails });
    const state = adminReducer(seeded, {
      type: deleteAlbumImage.fulfilled.type,
      payload: { albumId: 'a1', imageId: 'img1' },
    });
    expect(state.current?.images).toHaveLength(0);
  });

  it('updates coverImageId on set cover', () => {
    const seeded = adminReducer(undefined, { type: 'admin/albums/fetchOne/fulfilled', payload: albumDetails });
    const state = adminReducer(seeded, {
      type: setAlbumCover.fulfilled.type,
      payload: { albumId: 'a1', imageId: 'img1' },
    });
    expect(state.current?.coverImageId).toBe('img1');
  });

  it('clearCurrent wipes the loaded album', () => {
    const seeded = adminReducer(undefined, { type: 'admin/albums/fetchOne/fulfilled', payload: albumDetails });
    const state = adminReducer(seeded, clearCurrent());
    expect(state.current).toBeNull();
  });
});
