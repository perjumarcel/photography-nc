import { describe, expect, it, vi, beforeEach } from 'vitest';
import authReducer, { forceSignOut } from './authSlice';
import { login, logout, refresh } from '../api/thunks';
import type { AuthTokens } from './types';

const tokens: AuthTokens = {
  accessToken: 'access',
  accessTokenExpiresAt: '2026-01-01T00:00:00Z',
  refreshToken: 'refresh',
  refreshTokenExpiresAt: '2026-02-01T00:00:00Z',
  email: 'admin@example.com',
  role: 'Admin',
};

describe('authSlice', () => {
  beforeEach(() => {
    // Wipe the store between tests to keep `initialState` deterministic.
    localStorage.clear();
    vi.restoreAllMocks();
  });

  it('starts unauthenticated when localStorage is empty', () => {
    const state = authReducer(undefined, { type: '@@init' });
    expect(state.session).toBeNull();
    expect(state.status).toBe('idle');
  });

  it('login.fulfilled stores the session', () => {
    const state = authReducer(undefined, { type: login.fulfilled.type, payload: tokens });
    expect(state.session).toEqual({
      email: 'admin@example.com',
      role: 'Admin',
      accessTokenExpiresAt: '2026-01-01T00:00:00Z',
    });
    expect(state.status).toBe('succeeded');
  });

  it('login.rejected captures the error and clears any session', () => {
    const seeded = authReducer(undefined, { type: login.fulfilled.type, payload: tokens });
    const state = authReducer(seeded, { type: login.rejected.type, payload: 'bad creds' });
    expect(state.session).toBeNull();
    expect(state.error).toBe('bad creds');
  });

  it('refresh.rejected forces sign-out', () => {
    const seeded = authReducer(undefined, { type: login.fulfilled.type, payload: tokens });
    const state = authReducer(seeded, { type: refresh.rejected.type, payload: 'expired' });
    expect(state.session).toBeNull();
  });

  it('logout.fulfilled resets state', () => {
    const seeded = authReducer(undefined, { type: login.fulfilled.type, payload: tokens });
    const state = authReducer(seeded, { type: logout.fulfilled.type });
    expect(state.session).toBeNull();
    expect(state.status).toBe('idle');
  });

  it('forceSignOut clears the session', () => {
    const seeded = authReducer(undefined, { type: login.fulfilled.type, payload: tokens });
    const state = authReducer(seeded, forceSignOut());
    expect(state.session).toBeNull();
  });
});
