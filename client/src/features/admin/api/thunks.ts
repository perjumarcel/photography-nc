import { createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '@/shared/api/client';
import { extractErrorMessage } from '@/shared/lib/extractErrorMessage';
import type { AlbumDetailsDto, AlbumDto, ImageDto } from '@/features/albums/model/types';
import type { CategoryDto } from '@/features/categories/model/types';
import type { CreateAlbumDto, CreateCategoryDto, UpdateAlbumDto, UpdateCategoryDto } from '../model/types';

// ─── Albums ───────────────────────────────────────────────────────────────

export const fetchAdminAlbums = createAsyncThunk<AlbumDto[], void, { rejectValue: string }>(
  'admin/albums/fetch',
  async (_, { rejectWithValue }) => {
    try {
      const { data } = await api.get<AlbumDto[]>('/admin/albums');
      return data;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load albums'));
    }
  },
);

export const fetchAdminAlbum = createAsyncThunk<AlbumDetailsDto, string, { rejectValue: string }>(
  'admin/albums/fetchOne',
  async (id, { rejectWithValue }) => {
    try {
      const { data } = await api.get<AlbumDetailsDto>(`/admin/albums/${id}`);
      return data;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load album'));
    }
  },
);

export const createAdminAlbum = createAsyncThunk<{ id: string }, CreateAlbumDto, { rejectValue: string }>(
  'admin/albums/create',
  async (dto, { rejectWithValue }) => {
    try {
      const { data } = await api.post<{ id: string }>('/admin/albums', dto);
      return data;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to create album'));
    }
  },
);

interface UpdateAlbumPayload { id: string; dto: UpdateAlbumDto }
export const updateAdminAlbum = createAsyncThunk<string, UpdateAlbumPayload, { rejectValue: string }>(
  'admin/albums/update',
  async ({ id, dto }, { rejectWithValue }) => {
    try {
      await api.put(`/admin/albums/${id}`, dto);
      return id;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to update album'));
    }
  },
);

export const deleteAdminAlbum = createAsyncThunk<string, string, { rejectValue: string }>(
  'admin/albums/delete',
  async (id, { rejectWithValue }) => {
    try {
      await api.delete(`/admin/albums/${id}`);
      return id;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to delete album'));
    }
  },
);

// ─── Images (per album) ────────────────────────────────────────────────────

interface UploadImagePayload { albumId: string; file: File }
export const uploadAlbumImage = createAsyncThunk<ImageDto, UploadImagePayload, { rejectValue: string }>(
  'admin/albums/uploadImage',
  async ({ albumId, file }, { rejectWithValue, dispatch }) => {
    try {
      const form = new FormData();
      form.append('file', file);
      const { data } = await api.post<{ id: string }>(`/admin/albums/${albumId}/images`, form, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      // Re-fetch the album to pick up the new image with its server-computed dimensions.
      const refreshed = await dispatch(fetchAdminAlbum(albumId)).unwrap();
      const image = refreshed.images.find((i) => i.id === data.id);
      if (!image) throw new Error('Uploaded image not found in refreshed album');
      return image;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to upload image'));
    }
  },
);

interface ImageRefPayload { albumId: string; imageId: string }
export const deleteAlbumImage = createAsyncThunk<ImageRefPayload, ImageRefPayload, { rejectValue: string }>(
  'admin/albums/deleteImage',
  async ({ albumId, imageId }, { rejectWithValue }) => {
    try {
      await api.delete(`/admin/albums/${albumId}/images/${imageId}`);
      return { albumId, imageId };
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to delete image'));
    }
  },
);

export const setAlbumCover = createAsyncThunk<ImageRefPayload, ImageRefPayload, { rejectValue: string }>(
  'admin/albums/setCover',
  async ({ albumId, imageId }, { rejectWithValue }) => {
    try {
      await api.patch(`/admin/albums/${albumId}/images/${imageId}/cover`);
      return { albumId, imageId };
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to set cover'));
    }
  },
);

// ─── Categories ────────────────────────────────────────────────────────────

export const fetchAdminCategories = createAsyncThunk<CategoryDto[], void, { rejectValue: string }>(
  'admin/categories/fetch',
  async (_, { rejectWithValue }) => {
    try {
      const { data } = await api.get<CategoryDto[]>('/admin/categories');
      return data;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load categories'));
    }
  },
);

export const createAdminCategory = createAsyncThunk<{ id: number }, CreateCategoryDto, { rejectValue: string }>(
  'admin/categories/create',
  async (dto, { rejectWithValue }) => {
    try {
      const { data } = await api.post<{ id: number }>('/admin/categories', dto);
      return data;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to create category'));
    }
  },
);

interface UpdateCategoryPayload { id: number; dto: UpdateCategoryDto }
export const updateAdminCategory = createAsyncThunk<number, UpdateCategoryPayload, { rejectValue: string }>(
  'admin/categories/update',
  async ({ id, dto }, { rejectWithValue }) => {
    try {
      await api.put(`/admin/categories/${id}`, dto);
      return id;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to update category'));
    }
  },
);

export const deleteAdminCategory = createAsyncThunk<number, number, { rejectValue: string }>(
  'admin/categories/delete',
  async (id, { rejectWithValue }) => {
    try {
      await api.delete(`/admin/categories/${id}`);
      return id;
    } catch (err) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to delete category'));
    }
  },
);
