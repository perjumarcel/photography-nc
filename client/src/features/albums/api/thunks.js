import { createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '@/shared/api/client';
import { extractErrorMessage } from '@/shared/lib/extractErrorMessage';
export const fetchPublicAlbums = createAsyncThunk('albums/fetchPublic', async (_, { rejectWithValue }) => {
    try {
        const { data } = await api.get('/public/albums');
        return data;
    }
    catch (err) {
        return rejectWithValue(extractErrorMessage(err, 'Failed to load albums'));
    }
});
export const fetchAlbumById = createAsyncThunk('albums/fetchById', async (id, { rejectWithValue }) => {
    try {
        const { data } = await api.get(`/public/albums/${id}`);
        return data;
    }
    catch (err) {
        return rejectWithValue(extractErrorMessage(err, 'Failed to load album'));
    }
});
