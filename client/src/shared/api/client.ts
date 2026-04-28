import axios, { AxiosError, type AxiosRequestConfig } from 'axios';

/** Axios client used for every API call. Configures base URL, credentials, and 401 retry hook. */
export const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
  timeout: 30000,
});

let bearerToken: string | null = null;
export function setBearerToken(token: string | null): void {
  bearerToken = token;
}

/**
 * Hook that the auth feature wires up at startup. When a request returns 401
 * the interceptor calls this function to obtain a fresh access token, retries
 * the original request once, and falls through to the caller on a second
 * failure. Kept here (rather than directly importing the auth thunk) to keep
 * `shared/` free of any `features/` dependencies (FSD import rule).
 */
let refreshHandler: (() => Promise<string | null>) | null = null;
export function setRefreshHandler(fn: (() => Promise<string | null>) | null): void {
  refreshHandler = fn;
}

api.interceptors.request.use((config) => {
  if (bearerToken) {
    config.headers = config.headers ?? {};
    (config.headers as Record<string, string>).Authorization = `Bearer ${bearerToken}`;
  }
  return config;
});

interface RetriableConfig extends AxiosRequestConfig {
  _retried?: boolean;
}

api.interceptors.response.use(
  (resp) => resp,
  async (err: AxiosError) => {
    const original = err.config as RetriableConfig | undefined;
    const status = err.response?.status;
    const url = original?.url ?? '';

    // Don't try to refresh in response to the refresh/login/logout calls themselves.
    const isAuthCall = url.includes('/auth/');
    if (status === 401 && original && !original._retried && !isAuthCall && refreshHandler) {
      original._retried = true;
      try {
        const fresh = await refreshHandler();
        if (fresh) {
          original.headers = original.headers ?? {};
          (original.headers as Record<string, string>).Authorization = `Bearer ${fresh}`;
          return api.request(original);
        }
      } catch {
        /* fall through to throw original error */
      }
    }
    throw err;
  },
);
