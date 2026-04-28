import * as React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { Button } from '@/shared/ui/Button';
import {
  createAdminAlbum, deleteAdminAlbum, fetchAdminAlbums, fetchAdminCategories,
} from '@/features/admin/api/thunks';

/**
 * Admin albums list. Shows every album (including drafts) and offers a quick
 * inline form to create a new album. Each row links into the editor where the
 * operator can manage the album's images.
 */
export function AdminAlbumsPage(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const { albums, albumsStatus, categories, mutationStatus, mutationError } = useAppSelector((s) => ({
    albums: s.admin.albums,
    albumsStatus: s.admin.albumsStatus,
    categories: s.admin.categories,
    mutationStatus: s.admin.mutationStatus,
    mutationError: s.admin.mutationError,
  }));

  const [showForm, setShowForm] = useState(false);
  const [title, setTitle] = useState('');
  const [categoryId, setCategoryId] = useState<number | ''>('');

  useEffect(() => {
    void dispatch(fetchAdminAlbums());
    void dispatch(fetchAdminCategories());
  }, [dispatch]);

  // Default the category dropdown once categories arrive.
  useEffect(() => {
    if (categoryId === '' && categories.length > 0) setCategoryId(categories[0].id);
  }, [categories, categoryId]);

  const onCreate = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();
    if (!title.trim() || categoryId === '') return;
    const result = await dispatch(createAdminAlbum({ title: title.trim(), categoryId }));
    if (createAdminAlbum.fulfilled.match(result)) {
      setTitle('');
      setShowForm(false);
      void dispatch(fetchAdminAlbums());
    }
  };

  const onDelete = (id: string): void => {
    if (!window.confirm(t('admin.confirmDelete'))) return;
    void dispatch(deleteAdminAlbum(id));
  };

  return (
    <div>
      <header className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="font-display text-2xl text-ink">{t('admin.albums')}</h1>
        <Button type="button" variant="outline" onClick={() => setShowForm((v) => !v)}>
          {t('admin.newAlbum')}
        </Button>
      </header>

      {showForm && (
        <form onSubmit={onCreate} className="mt-6 grid gap-4 rounded-xl border border-zinc-200 bg-paper p-5 sm:grid-cols-[1fr_auto_auto]">
          <input
            type="text"
            placeholder={t('admin.title')}
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
          />
          <select
            value={categoryId}
            onChange={(e) => setCategoryId(Number(e.target.value))}
            required
            className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
          >
            {categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>
          <Button type="submit" loading={mutationStatus === 'loading'}>{t('admin.save')}</Button>
        </form>
      )}

      {mutationError && (
        <p role="alert" className="mt-3 text-sm text-red-600">{mutationError}</p>
      )}

      <div className="mt-6 overflow-x-auto rounded-xl border border-zinc-200 bg-paper">
        <table className="min-w-full divide-y divide-zinc-200 text-sm">
          <thead className="bg-paper-soft text-left text-xs uppercase tracking-[0.2em] text-ink/60">
            <tr>
              <th className="px-4 py-3">{t('admin.title')}</th>
              <th className="px-4 py-3">{t('admin.category')}</th>
              <th className="px-4 py-3">{t('admin.images')}</th>
              <th className="px-4 py-3" />
            </tr>
          </thead>
          <tbody className="divide-y divide-zinc-100">
            {albumsStatus === 'loading' && (
              <tr><td colSpan={4} className="px-4 py-6 text-center text-ink/60">{t('common.loading')}</td></tr>
            )}
            {albumsStatus === 'succeeded' && albums.length === 0 && (
              <tr><td colSpan={4} className="px-4 py-6 text-center text-ink/60">{t('common.empty')}</td></tr>
            )}
            {albums.map((a) => (
              <tr key={a.id}>
                <td className="px-4 py-3 text-ink">
                  <Link to={`/admin/albums/${a.id}`} className="hover:text-brand">{a.title}</Link>
                </td>
                <td className="px-4 py-3 text-ink/70">
                  {categories.find((c) => c.id === a.categoryId)?.name ?? a.categoryId}
                </td>
                <td className="px-4 py-3 text-ink/70">{a.imageCount}</td>
                <td className="px-4 py-3 text-right">
                  <button
                    type="button"
                    onClick={() => onDelete(a.id)}
                    className="text-xs uppercase tracking-[0.2em] text-red-600 hover:text-red-700"
                  >
                    {t('admin.delete')}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
