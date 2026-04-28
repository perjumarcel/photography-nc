/** Convert any thrown value into a user-facing string. Never use `err: any`. */
export function extractErrorMessage(err: unknown, fallback = 'Unexpected error'): string {
  if (typeof err === 'string') return err;
  if (err && typeof err === 'object') {
    const maybe = err as { response?: { data?: { error?: string; message?: string } }; message?: string };
    return (
      maybe.response?.data?.error ??
      maybe.response?.data?.message ??
      maybe.message ??
      fallback
    );
  }
  return fallback;
}
