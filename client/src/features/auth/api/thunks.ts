import { createAsyncThunk } from '@reduxjs/toolkit';
import { api, setBearerToken } from '@/shared/api/client';
import { extractErrorMessage } from '@/shared/lib/extractErrorMessage';
import { tokenStore } from '../lib/tokenStore';
import type { AuthTokens } from '../model/types';

interface LoginPayload {
  email: string;
  password: string;
}

/** POST `/api/auth/login` and persist the returned tokens. */
export const login = createAsyncThunk<AuthTokens, LoginPayload, { rejectValue: string }>(
  'auth/login',
  async (payload, { rejectWithValue }) => {
    try {
      const { data } = await api.post<AuthTokens>('/auth/login', payload);
      tokenStore.save(data);
      setBearerToken(data.accessToken);
      return data;
    } catch (err: unknown) {
      return rejectWithValue(extractErrorMessage(err, 'Login failed'));
    }
  },
);

/** POST `/api/auth/refresh` with the current refresh token; rotates both tokens. */
export const refresh = createAsyncThunk<AuthTokens, void, { rejectValue: string }>(
  'auth/refresh',
  async (_, { rejectWithValue }) => {
    const stored = tokenStore.load();
    if (!stored) return rejectWithValue('No refresh token available');
    try {
      const { data } = await api.post<AuthTokens>('/auth/refresh', {
        refreshToken: stored.refreshToken,
      });
      tokenStore.save(data);
      setBearerToken(data.accessToken);
      return data;
    } catch (err: unknown) {
      tokenStore.clear();
      setBearerToken(null);
      return rejectWithValue(extractErrorMessage(err, 'Session expired'));
    }
  },
);

/** POST `/api/auth/logout` (best-effort) and clear local state regardless. */
export const logout = createAsyncThunk<void, void>(
  'auth/logout',
  async () => {
    const stored = tokenStore.load();
    try {
      if (stored) {
        await api.post('/auth/logout', { refreshToken: stored.refreshToken });
      }
    } catch {
      /* logout is best-effort — clear local state even if the call fails. */
    } finally {
      tokenStore.clear();
      setBearerToken(null);
    }
  },
);
