import { createSlice } from '@reduxjs/toolkit';
import { tokenStore } from '../lib/tokenStore';
import { login, logout, refresh } from '../api/thunks';
import type { AuthState, AuthTokens } from './types';

function sessionFromTokens(t: AuthTokens) {
  return { email: t.email, role: t.role, accessTokenExpiresAt: t.accessTokenExpiresAt };
}

const persisted = tokenStore.load();

const initialState: AuthState = {
  session: persisted ? sessionFromTokens(persisted) : null,
  status: 'idle',
  error: null,
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    /** Forces a sign-out from the client side without hitting the network. */
    forceSignOut(state) {
      state.session = null;
      state.error = null;
      tokenStore.clear();
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.status = 'loading';
        state.error = null;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.session = sessionFromTokens(action.payload);
      })
      .addCase(login.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload ?? 'Login failed';
        state.session = null;
      })
      .addCase(refresh.fulfilled, (state, action) => {
        state.session = sessionFromTokens(action.payload);
      })
      .addCase(refresh.rejected, (state, action) => {
        state.session = null;
        state.error = action.payload ?? 'Session expired';
      })
      .addCase(logout.fulfilled, (state) => {
        state.session = null;
        state.status = 'idle';
        state.error = null;
      });
  },
});

export const { forceSignOut } = authSlice.actions;
export default authSlice.reducer;
