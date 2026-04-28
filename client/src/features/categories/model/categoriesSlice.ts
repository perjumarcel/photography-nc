import { createSlice } from '@reduxjs/toolkit';
import { fetchPublicCategories } from '../api/thunks';
import type { CategoryDto } from './types';
import type { LoadStatus } from '@/features/albums/model/types';

interface CategoriesState {
  list: CategoryDto[];
  status: LoadStatus;
  error: string | null;
}

const initialState: CategoriesState = {
  list: [],
  status: 'idle',
  error: null,
};

export const categoriesSlice = createSlice({
  name: 'categories',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchPublicCategories.pending, (state) => {
        state.status = 'loading';
        state.error = null;
      })
      .addCase(fetchPublicCategories.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.list = action.payload;
      })
      .addCase(fetchPublicCategories.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload ?? 'Failed to load categories';
      });
  },
});

export default categoriesSlice.reducer;
