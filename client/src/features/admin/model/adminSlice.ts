import { createSlice, isAnyOf, type PayloadAction } from '@reduxjs/toolkit';
import {
  createAdminAlbum, createAdminCategory, deleteAdminAlbum, deleteAdminCategory,
  deleteAlbumImage, fetchAdminAlbum, fetchAdminAlbums, fetchAdminCategories,
  setAlbumCover, updateAdminAlbum, updateAdminCategory, uploadAlbumImage,
} from '../api/thunks';
import type { AdminState } from './types';

const initialState: AdminState = {
  albums: [],
  albumsStatus: 'idle',
  albumsError: null,
  categories: [],
  categoriesStatus: 'idle',
  categoriesError: null,
  current: null,
  currentStatus: 'idle',
  currentError: null,
  mutationStatus: 'idle',
  mutationError: null,
};

export const adminSlice = createSlice({
  name: 'admin',
  initialState,
  reducers: {
    /** Resets `mutationStatus` so previous toasts/errors don't bleed across pages. */
    resetMutation(state) {
      state.mutationStatus = 'idle';
      state.mutationError = null;
    },
    clearCurrent(state) {
      state.current = null;
      state.currentStatus = 'idle';
      state.currentError = null;
    },
  },
  extraReducers: (builder) => {
    // ── Albums list ────────────────────────────────────────────────────────
    builder
      .addCase(fetchAdminAlbums.pending, (state) => {
        state.albumsStatus = 'loading';
        state.albumsError = null;
      })
      .addCase(fetchAdminAlbums.fulfilled, (state, action) => {
        state.albumsStatus = 'succeeded';
        state.albums = action.payload;
      })
      .addCase(fetchAdminAlbums.rejected, (state, action) => {
        state.albumsStatus = 'failed';
        state.albumsError = action.payload ?? 'Failed to load albums';
      })

    // ── Single album ───────────────────────────────────────────────────────
      .addCase(fetchAdminAlbum.pending, (state) => {
        state.currentStatus = 'loading';
        state.currentError = null;
      })
      .addCase(fetchAdminAlbum.fulfilled, (state, action) => {
        state.currentStatus = 'succeeded';
        state.current = action.payload;
      })
      .addCase(fetchAdminAlbum.rejected, (state, action) => {
        state.currentStatus = 'failed';
        state.currentError = action.payload ?? 'Failed to load album';
      })

      .addCase(deleteAdminAlbum.fulfilled, (state, action: PayloadAction<string>) => {
        state.albums = state.albums.filter((a) => a.id !== action.payload);
        if (state.current?.id === action.payload) state.current = null;
      })

    // ── Categories ─────────────────────────────────────────────────────────
      .addCase(fetchAdminCategories.pending, (state) => {
        state.categoriesStatus = 'loading';
        state.categoriesError = null;
      })
      .addCase(fetchAdminCategories.fulfilled, (state, action) => {
        state.categoriesStatus = 'succeeded';
        state.categories = action.payload;
      })
      .addCase(fetchAdminCategories.rejected, (state, action) => {
        state.categoriesStatus = 'failed';
        state.categoriesError = action.payload ?? 'Failed to load categories';
      })
      .addCase(deleteAdminCategory.fulfilled, (state, action: PayloadAction<number>) => {
        state.categories = state.categories.filter((c) => c.id !== action.payload);
      })

    // ── Image management within current album ──────────────────────────────
      .addCase(deleteAlbumImage.fulfilled, (state, action) => {
        if (state.current?.id === action.payload.albumId) {
          state.current = {
            ...state.current,
            images: state.current.images.filter((i) => i.id !== action.payload.imageId),
          };
        }
      })
      .addCase(setAlbumCover.fulfilled, (state, action) => {
        if (state.current?.id === action.payload.albumId) {
          state.current = { ...state.current, coverImageId: action.payload.imageId };
        }
      })

    // ── Generic mutation status (covers all create/update/upload/delete flows) ──
      .addMatcher(
        isAnyOf(
          createAdminAlbum.pending, updateAdminAlbum.pending, deleteAdminAlbum.pending,
          uploadAlbumImage.pending, deleteAlbumImage.pending, setAlbumCover.pending,
          createAdminCategory.pending, updateAdminCategory.pending, deleteAdminCategory.pending,
        ),
        (state) => {
          state.mutationStatus = 'loading';
          state.mutationError = null;
        },
      )
      .addMatcher(
        isAnyOf(
          createAdminAlbum.fulfilled, updateAdminAlbum.fulfilled, deleteAdminAlbum.fulfilled,
          uploadAlbumImage.fulfilled, deleteAlbumImage.fulfilled, setAlbumCover.fulfilled,
          createAdminCategory.fulfilled, updateAdminCategory.fulfilled, deleteAdminCategory.fulfilled,
        ),
        (state) => { state.mutationStatus = 'succeeded'; },
      )
      .addMatcher(
        isAnyOf(
          createAdminAlbum.rejected, updateAdminAlbum.rejected, deleteAdminAlbum.rejected,
          uploadAlbumImage.rejected, deleteAlbumImage.rejected, setAlbumCover.rejected,
          createAdminCategory.rejected, updateAdminCategory.rejected, deleteAdminCategory.rejected,
        ),
        (state, action) => {
          state.mutationStatus = 'failed';
          state.mutationError = (action.payload as string | undefined) ?? 'Operation failed';
        },
      );
  },
});

export const { resetMutation, clearCurrent } = adminSlice.actions;
export default adminSlice.reducer;
