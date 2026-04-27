import { setBearerToken, setRefreshHandler } from '@/shared/api/client';
import { tokenStore } from './tokenStore';
import { store } from '@/app/store';
import { forceSignOut } from '../model/authSlice';
import { refresh } from '../api/thunks';

/**
 * Wires the persisted access token + the auto-refresh interceptor on app start.
 *
 * Called once from `main.tsx`. We intentionally avoid touching the network on
 * boot: refresh is only attempted lazily, when an authenticated request fails.
 */
export function initAuth(): void {
  const stored = tokenStore.load();
  if (stored) setBearerToken(stored.accessToken);

  setRefreshHandler(async () => {
    const result = await store.dispatch(refresh());
    if (refresh.fulfilled.match(result)) return result.payload.accessToken;
    store.dispatch(forceSignOut());
    return null;
  });
}
