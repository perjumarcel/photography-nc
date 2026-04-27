import axios from 'axios';
/** Axios client used for every API call. Configures base URL, credentials, and 401 retry hook. */
export const api = axios.create({
    baseURL: '/api',
    withCredentials: true,
    timeout: 30000,
});
let bearerToken = null;
export function setBearerToken(token) {
    bearerToken = token;
}
api.interceptors.request.use((config) => {
    if (bearerToken) {
        config.headers = config.headers ?? {};
        config.headers.Authorization = `Bearer ${bearerToken}`;
    }
    return config;
});
api.interceptors.response.use((resp) => resp, async (err) => {
    if (err?.response?.status === 401 || err?.response?.status === 403) {
        // Hook for refresh-token rotation. Real implementation lives in features/auth.
    }
    throw err;
});
