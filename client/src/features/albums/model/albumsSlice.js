import { createSlice } from '@reduxjs/toolkit';
import { fetchAlbumById, fetchPublicAlbums } from '../api/thunks';
const initialState = {
    list: [],
    listStatus: 'idle',
    listError: null,
    current: null,
    currentStatus: 'idle',
    currentError: null,
};
export const albumsSlice = createSlice({
    name: 'albums',
    initialState,
    reducers: {
        clearCurrent(state) {
            state.current = null;
            state.currentStatus = 'idle';
            state.currentError = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchPublicAlbums.pending, (state) => {
            state.listStatus = 'loading';
            state.listError = null;
        })
            .addCase(fetchPublicAlbums.fulfilled, (state, action) => {
            state.listStatus = 'succeeded';
            state.list = action.payload;
        })
            .addCase(fetchPublicAlbums.rejected, (state, action) => {
            state.listStatus = 'failed';
            state.listError = action.payload ?? 'Failed to load albums';
        })
            .addCase(fetchAlbumById.pending, (state) => {
            state.currentStatus = 'loading';
            state.currentError = null;
        })
            .addCase(fetchAlbumById.fulfilled, (state, action) => {
            state.currentStatus = 'succeeded';
            state.current = action.payload;
        })
            .addCase(fetchAlbumById.rejected, (state, action) => {
            state.currentStatus = 'failed';
            state.currentError = action.payload ?? 'Failed to load album';
        });
    },
});
export const { clearCurrent } = albumsSlice.actions;
export default albumsSlice.reducer;
