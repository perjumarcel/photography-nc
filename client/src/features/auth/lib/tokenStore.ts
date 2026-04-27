import type { AuthTokens } from '../model/types';

const ACCESS_KEY = 'pnc.accessToken';
const ACCESS_EXP_KEY = 'pnc.accessTokenExpiresAt';
const REFRESH_KEY = 'pnc.refreshToken';
const REFRESH_EXP_KEY = 'pnc.refreshTokenExpiresAt';
const EMAIL_KEY = 'pnc.email';
const ROLE_KEY = 'pnc.role';

/**
 * Centralised access to persisted JWT + refresh tokens.
 *
 * We use `localStorage` rather than `httpOnly` cookies because the API also
 * needs to be callable from non-browser clients in the future, and because
 * the React client and API are deployed under different origins. To mitigate
 * XSS exposure, the refresh token is **single-use** and rotated by the
 * backend on every refresh call.
 */
export const tokenStore = {
  load(): AuthTokens | null {
    try {
      const accessToken = localStorage.getItem(ACCESS_KEY);
      const refreshToken = localStorage.getItem(REFRESH_KEY);
      const accessTokenExpiresAt = localStorage.getItem(ACCESS_EXP_KEY);
      const refreshTokenExpiresAt = localStorage.getItem(REFRESH_EXP_KEY);
      const email = localStorage.getItem(EMAIL_KEY);
      const role = localStorage.getItem(ROLE_KEY);
      if (!accessToken || !refreshToken || !accessTokenExpiresAt || !refreshTokenExpiresAt || !email || !role) {
        return null;
      }
      return { accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt, email, role };
    } catch {
      return null; // localStorage may throw in privacy mode / SSR
    }
  },
  save(tokens: AuthTokens): void {
    try {
      localStorage.setItem(ACCESS_KEY, tokens.accessToken);
      localStorage.setItem(ACCESS_EXP_KEY, tokens.accessTokenExpiresAt);
      localStorage.setItem(REFRESH_KEY, tokens.refreshToken);
      localStorage.setItem(REFRESH_EXP_KEY, tokens.refreshTokenExpiresAt);
      localStorage.setItem(EMAIL_KEY, tokens.email);
      localStorage.setItem(ROLE_KEY, tokens.role);
    } catch {
      /* no-op */
    }
  },
  clear(): void {
    try {
      [ACCESS_KEY, ACCESS_EXP_KEY, REFRESH_KEY, REFRESH_EXP_KEY, EMAIL_KEY, ROLE_KEY].forEach((k) =>
        localStorage.removeItem(k),
      );
    } catch {
      /* no-op */
    }
  },
};
