import * as React from 'react';
import { NavLink, Outlet } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { logout } from '@/features/auth/api/thunks';
import { cn } from '@/shared/lib/cn';

/**
 * Chrome for the `/admin/*` routes. A persistent sidebar (collapses to a top
 * bar on small screens) hosts navigation between Dashboard, Albums and
 * Categories, plus a sign-out action that calls `/api/auth/logout`.
 */
export function AdminLayout(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const session = useAppSelector((s) => s.auth.session);

  const linkClass = ({ isActive }: { isActive: boolean }): string =>
    cn(
      'block rounded-lg px-3 py-2 text-sm font-medium transition-colors',
      isActive ? 'bg-ink text-paper' : 'text-ink/70 hover:bg-paper-soft hover:text-ink',
    );

  return (
    <div className="min-h-screen bg-paper-soft text-ink lg:grid lg:grid-cols-[16rem_1fr]">
      <aside className="border-b border-zinc-200 bg-paper p-4 lg:min-h-screen lg:border-b-0 lg:border-r">
        <div className="mb-6">
          <p className="text-[0.7rem] uppercase tracking-[0.4em] text-ink/60">{t('app.brand')}</p>
          <p className="mt-1 font-display text-lg text-ink">{t('admin.dashboard')}</p>
        </div>
        <nav aria-label={t('admin.dashboard')} className="space-y-1">
          <NavLink to="/admin" end className={linkClass}>{t('admin.dashboard')}</NavLink>
          <NavLink to="/admin/albums" className={linkClass}>{t('admin.albums')}</NavLink>
          <NavLink to="/admin/categories" className={linkClass}>{t('admin.categories')}</NavLink>
        </nav>
        <div className="mt-8 space-y-2 border-t border-zinc-200 pt-4 text-sm">
          {session && <p className="text-ink/60">{session.email}</p>}
          <button
            type="button"
            onClick={() => { void dispatch(logout()); }}
            className="text-xs uppercase tracking-[0.2em] text-ink/70 hover:text-ink"
          >
            {t('admin.signOut')}
          </button>
        </div>
      </aside>

      <main className="p-6 sm:p-8">
        <Outlet />
      </main>
    </div>
  );
}
