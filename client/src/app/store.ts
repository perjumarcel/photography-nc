import { configureStore } from '@reduxjs/toolkit';
import albumsReducer from '@/features/albums/model/albumsSlice';
import categoriesReducer from '@/features/categories/model/categoriesSlice';
import contactReducer from '@/features/contact/model/contactSlice';
import authReducer from '@/features/auth/model/authSlice';
import adminReducer from '@/features/admin/model/adminSlice';

export const store = configureStore({
  reducer: {
    albums: albumsReducer,
    categories: categoriesReducer,
    contact: contactReducer,
    auth: authReducer,
    admin: adminReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
