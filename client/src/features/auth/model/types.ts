import type { LoadStatus } from '@/features/albums/model/types';

/** Backend response payload from `/api/auth/login` and `/api/auth/refresh`. */
export interface AuthTokens {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
  email: string;
  role: string;
}

export interface AuthState {
  /** Decoded session derived from the issued tokens, or `null` when signed out. */
  session: {
    email: string;
    role: string;
    accessTokenExpiresAt: string;
  } | null;
  status: LoadStatus;
  error: string | null;
}
