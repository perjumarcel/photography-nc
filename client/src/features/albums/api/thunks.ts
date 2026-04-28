import { createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '@/shared/api/client';
import { extractErrorMessage } from '@/shared/lib/extractErrorMessage';
import type { AlbumDetailsDto, AlbumDto } from '../model/types';

export const fetchPublicAlbums = createAsyncThunk<AlbumDto[], void, { rejectValue: string }>(
  'albums/fetchPublic',
  async (_, { rejectWithValue }) => {
    try {
      const { data } = await api.get<AlbumDto[]>('/public/albums');
      return data;
    } catch (err: unknown) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load albums'));
    }
  },
);

export const fetchAlbumById = createAsyncThunk<AlbumDetailsDto, string, { rejectValue: string }>(
  'albums/fetchById',
  async (id, { rejectWithValue }) => {
    try {
      const { data } = await api.get<AlbumDetailsDto>(`/public/albums/${id}`);
      return data;
    } catch (err: unknown) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load album'));
    }
  },
);
