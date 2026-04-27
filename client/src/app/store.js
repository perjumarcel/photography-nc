import { configureStore } from '@reduxjs/toolkit';
import albumsReducer from '@/features/albums/model/albumsSlice';
export const store = configureStore({
    reducer: {
        albums: albumsReducer,
    },
});
