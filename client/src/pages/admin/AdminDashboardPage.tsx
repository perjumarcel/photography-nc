import * as React from 'react';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { fetchAdminAlbums, fetchAdminCategories } from '@/features/admin/api/thunks';

/**
 * Admin landing page — shows a high-level summary (counts) so the operator has
 * a quick situational overview before drilling into albums or categories.
 */
export function AdminDashboardPage(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const { albums, categories, session } = useAppSelector((s) => ({
    albums: s.admin.albums,
    categories: s.admin.categories,
    session: s.auth.session,
  }));

  useEffect(() => {
    void dispatch(fetchAdminAlbums());
    void dispatch(fetchAdminCategories());
  }, [dispatch]);

  const cards = [
    { label: t('admin.albums'), value: albums.length },
    { label: t('admin.categories'), value: categories.length },
  ];

  return (
    <div>
      <h1 className="font-display text-2xl text-ink">{t('admin.dashboard')}</h1>
      {session && <p className="mt-2 text-sm text-ink/60">{t('admin.welcome', { email: session.email })}</p>}

      <div className="mt-8 grid gap-4 sm:grid-cols-2">
        {cards.map((c) => (
          <div key={c.label} className="rounded-xl border border-zinc-200 bg-paper p-5">
            <p className="text-xs uppercase tracking-[0.2em] text-ink/60">{c.label}</p>
            <p className="mt-2 font-display text-3xl text-ink">{c.value}</p>
          </div>
        ))}
      </div>
    </div>
  );
}
