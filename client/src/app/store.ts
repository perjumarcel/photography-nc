import { configureStore } from '@reduxjs/toolkit';
import albumsReducer from '@/features/albums/model/albumsSlice';
import categoriesReducer from '@/features/categories/model/categoriesSlice';

export const store = configureStore({
  reducer: {
    albums: albumsReducer,
    categories: categoriesReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
