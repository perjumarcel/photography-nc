/** Convert any thrown value into a user-facing string. Never use `err: any`. */
export function extractErrorMessage(err, fallback = 'Unexpected error') {
    if (typeof err === 'string')
        return err;
    if (err && typeof err === 'object') {
        const maybe = err;
        return (maybe.response?.data?.error ??
            maybe.response?.data?.message ??
            maybe.message ??
            fallback);
    }
    return fallback;
}
