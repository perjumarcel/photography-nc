import * as React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { Button } from '@/shared/ui/Button';
import { login } from '../api/thunks';

interface LocationState { from?: { pathname?: string } }

/**
 * Admin sign-in page. On success bounces back to the originally-requested URL
 * (captured by `RequireAuth`) or `/admin` if the user landed here directly.
 */
export function LoginPage(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { status, error, session } = useAppSelector((s) => s.auth);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const target = (location.state as LocationState | null)?.from?.pathname ?? '/admin';

  useEffect(() => {
    if (session) navigate(target, { replace: true });
  }, [session, navigate, target]);

  const onSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();
    await dispatch(login({ email, password }));
  };

  const isSubmitting = status === 'loading';

  return (
    <main className="mx-auto flex min-h-screen w-full max-w-md flex-col justify-center px-6 py-12">
      <h1 className="font-display text-3xl font-light text-ink">{t('auth.title')}</h1>
      <p className="mt-2 text-sm text-ink/60">{t('auth.subtitle')}</p>

      <form onSubmit={onSubmit} noValidate className="mt-8 space-y-5">
        <div>
          <label htmlFor="login-email" className="block text-[0.7rem] uppercase tracking-[0.25em] text-ink/70">
            {t('auth.email')}
          </label>
          <input
            id="login-email"
            name="email"
            type="email"
            autoComplete="username"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            disabled={isSubmitting}
            className="mt-2 w-full rounded-none border border-ink/20 bg-paper px-3 py-2.5 text-ink focus:border-ink focus:outline-none disabled:opacity-60"
          />
        </div>
        <div>
          <label htmlFor="login-password" className="block text-[0.7rem] uppercase tracking-[0.25em] text-ink/70">
            {t('auth.password')}
          </label>
          <input
            id="login-password"
            name="password"
            type="password"
            autoComplete="current-password"
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={isSubmitting}
            className="mt-2 w-full rounded-none border border-ink/20 bg-paper px-3 py-2.5 text-ink focus:border-ink focus:outline-none disabled:opacity-60"
          />
        </div>

        {error && (
          <p role="alert" className="text-sm text-red-600">
            {error}
          </p>
        )}

        <Button type="submit" loading={isSubmitting} className="w-full">
          {t('auth.signIn')}
        </Button>
      </form>
    </main>
  );
}
