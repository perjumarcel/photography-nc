import { describe, expect, it } from 'vitest';
import albumsReducer, { clearCurrent } from './albumsSlice';
import { fetchPublicAlbums } from '../api/thunks';
import type { AlbumDto } from './types';

const sample: AlbumDto = {
  id: '00000000-0000-0000-0000-000000000001',
  title: 'Sample',
  slug: 'sample',
  seoTitle: 'Sample SEO',
  seoDescription: 'Sample SEO description',
  coverAltText: 'Sample cover',
  showInPortfolio: true,
  showInStories: false,
  showInHome: false,
  categoryId: 1,
  imageCount: 0,
  coverPublicUrl: '/cover.jpg',
  coverWidth: 100,
  coverHeight: 80,
  coverVariants: {
    placeholder: '/cover.jpg?width=40',
    thumbnail: '/cover.jpg?width=240',
    card: '/cover.jpg?width=640',
    hero: '/cover.jpg?width=1600',
    full: '/cover.jpg',
  },
};

describe('albumsSlice', () => {
  it('starts idle', () => {
    const state = albumsReducer(undefined, { type: '@@init' });
    expect(state.listStatus).toBe('idle');
    expect(state.list).toEqual([]);
  });

  it('handles fetchPublicAlbums.fulfilled', () => {
    const state = albumsReducer(undefined, {
      type: fetchPublicAlbums.fulfilled.type,
      payload: [sample],
    });
    expect(state.listStatus).toBe('succeeded');
    expect(state.list).toHaveLength(1);
  });

  it('handles fetchPublicAlbums.rejected', () => {
    const state = albumsReducer(undefined, {
      type: fetchPublicAlbums.rejected.type,
      payload: 'boom',
    });
    expect(state.listStatus).toBe('failed');
    expect(state.listError).toBe('boom');
  });

  it('clearCurrent resets the current album', () => {
    const seeded = albumsReducer(undefined, {
      type: 'albums/fetchById/fulfilled',
      payload: { ...sample, images: [] },
    });
    const cleared = albumsReducer(seeded, clearCurrent());
    expect(cleared.current).toBeNull();
    expect(cleared.currentStatus).toBe('idle');
  });
});
