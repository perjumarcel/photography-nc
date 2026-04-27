import { createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '@/shared/api/client';
import { extractErrorMessage } from '@/shared/lib/extractErrorMessage';
import type { CategoryDto } from '../model/types';

export const fetchPublicCategories = createAsyncThunk<CategoryDto[], void, { rejectValue: string }>(
  'categories/fetchPublic',
  async (_, { rejectWithValue }) => {
    try {
      const { data } = await api.get<CategoryDto[]>('/public/categories');
      return data;
    } catch (err: unknown) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load categories'));
    }
  },
);
