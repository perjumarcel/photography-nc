import * as React from 'react';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { Button } from '@/shared/ui/Button';
import {
  deleteAlbumImage, fetchAdminAlbum, fetchAdminCategories, setAlbumCover,
  updateAdminAlbum, uploadAlbumImage,
} from '@/features/admin/api/thunks';
import { clearCurrent } from '@/features/admin/model/adminSlice';

/**
 * Edit a single album: title/category/visibility flags, plus image upload,
 * cover selection and image deletion. Mounting fetches the full album details
 * (including the image list with public URLs); unmount clears `current` so
 * subsequent admin screens start from a clean slate.
 */
export function AdminAlbumEditPage(): React.JSX.Element {
  const { t } = useTranslation();
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const fileInput = useRef<HTMLInputElement>(null);

  const { album, categories, mutationStatus, mutationError, currentStatus } = useAppSelector((s) => ({
    album: s.admin.current,
    categories: s.admin.categories,
    mutationStatus: s.admin.mutationStatus,
    mutationError: s.admin.mutationError,
    currentStatus: s.admin.currentStatus,
  }));

  // Local form state mirrors the loaded album.
  const [form, setForm] = useState({
    title: '', categoryId: 0, description: '',
    eventDate: '', client: '', location: '',
    showInPortfolio: false, showInStories: false, showInHome: false,
  });

  useEffect(() => {
    if (!id) return;
    void dispatch(fetchAdminAlbum(id));
    void dispatch(fetchAdminCategories());
    return () => { dispatch(clearCurrent()); };
  }, [id, dispatch]);

  // Sync server -> form when the album loads or changes id.
  useEffect(() => {
    if (album && album.id === id) {
      setForm({
        title: album.title,
        categoryId: album.categoryId,
        description: album.description ?? '',
        eventDate: album.eventDate ? album.eventDate.slice(0, 10) : '',
        client: album.client ?? '',
        location: album.location ?? '',
        showInPortfolio: album.showInPortfolio,
        showInStories: album.showInStories,
        showInHome: album.showInHome,
      });
    }
  }, [album, id]);

  if (!id) return <p className="text-ink/60">Missing album id.</p>;

  const onSave = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();
    await dispatch(updateAdminAlbum({
      id,
      dto: {
        title: form.title.trim(),
        categoryId: form.categoryId,
        description: form.description || null,
        eventDate: form.eventDate ? new Date(form.eventDate).toISOString() : null,
        client: form.client || null,
        location: form.location || null,
        showInPortfolio: form.showInPortfolio,
        showInStories: form.showInStories,
        showInHome: form.showInHome,
      },
    }));
  };

  const onPickFile = (e: React.ChangeEvent<HTMLInputElement>): void => {
    const file = e.target.files?.[0];
    if (!file) return;
    void dispatch(uploadAlbumImage({ albumId: id, file })).then(() => {
      if (fileInput.current) fileInput.current.value = '';
    });
  };

  return (
    <div>
      <Link to="/admin/albums" className="text-xs uppercase tracking-[0.2em] text-ink/60 hover:text-ink">
        ← {t('admin.albums')}
      </Link>
      <h1 className="mt-3 font-display text-2xl text-ink">
        {currentStatus === 'loading' ? t('common.loading') : (album?.title ?? '')}
      </h1>

      {mutationError && (
        <p role="alert" className="mt-3 text-sm text-red-600">{mutationError}</p>
      )}

      {album && (
        <form onSubmit={onSave} className="mt-6 grid gap-4 rounded-xl border border-zinc-200 bg-paper p-5 sm:grid-cols-2">
          <Field label={t('admin.title')}>
            <input
              type="text" required value={form.title}
              onChange={(e) => setForm({ ...form, title: e.target.value })}
              className={inputCls}
            />
          </Field>
          <Field label={t('admin.category')}>
            <select
              value={form.categoryId}
              onChange={(e) => setForm({ ...form, categoryId: Number(e.target.value) })}
              className={inputCls}
            >
              {categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </Field>
          <Field label={t('admin.eventDate')}>
            <input
              type="date" value={form.eventDate}
              onChange={(e) => setForm({ ...form, eventDate: e.target.value })}
              className={inputCls}
            />
          </Field>
          <Field label={t('admin.client')}>
            <input
              type="text" value={form.client}
              onChange={(e) => setForm({ ...form, client: e.target.value })}
              className={inputCls}
            />
          </Field>
          <Field label={t('admin.location')} className="sm:col-span-2">
            <input
              type="text" value={form.location}
              onChange={(e) => setForm({ ...form, location: e.target.value })}
              className={inputCls}
            />
          </Field>
          <Field label={t('admin.description')} className="sm:col-span-2">
            <textarea
              rows={4} value={form.description}
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              className={inputCls}
            />
          </Field>

          <div className="flex flex-col gap-2 sm:col-span-2">
            <Checkbox label={t('admin.showInPortfolio')} checked={form.showInPortfolio}
              onChange={(v) => setForm({ ...form, showInPortfolio: v })} />
            <Checkbox label={t('admin.showInStories')} checked={form.showInStories}
              onChange={(v) => setForm({ ...form, showInStories: v })} />
            <Checkbox label={t('admin.showInHome')} checked={form.showInHome}
              onChange={(v) => setForm({ ...form, showInHome: v })} />
          </div>

          <div className="sm:col-span-2">
            <Button type="submit" loading={mutationStatus === 'loading'}>{t('admin.save')}</Button>
          </div>
        </form>
      )}

      {album && (
        <section className="mt-8">
          <header className="flex items-center justify-between">
            <h2 className="font-display text-xl text-ink">{t('admin.images')}</h2>
            <label className="cursor-pointer rounded-lg border border-ink px-4 py-2 text-xs uppercase tracking-[0.2em] text-ink hover:bg-ink hover:text-paper">
              {t('admin.upload')}
              <input
                ref={fileInput}
                type="file"
                accept="image/*"
                className="sr-only"
                onChange={onPickFile}
              />
            </label>
          </header>

          {mutationStatus === 'loading' && (
            <p className="mt-3 text-xs text-ink/60">{t('admin.uploading')}</p>
          )}

          {album.images.length === 0 ? (
            <p className="mt-6 rounded-xl border border-dashed border-zinc-300 bg-paper p-10 text-center text-ink/60">
              {t('admin.noImages')}
            </p>
          ) : (
            <ul className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {album.images.map((img) => {
                const isCover = album.coverImageId === img.id;
                return (
                  <li key={img.id} className="overflow-hidden rounded-xl border border-zinc-200 bg-paper">
                    <img
                      src={img.publicUrl}
                      alt={img.originalName}
                      width={img.width}
                      height={img.height}
                      loading="lazy"
                      className="aspect-[4/3] w-full object-cover"
                    />
                    <div className="flex items-center justify-between gap-2 p-3 text-xs">
                      <span className="truncate text-ink/70" title={img.originalName}>{img.originalName}</span>
                      <div className="flex gap-2 whitespace-nowrap">
                        <button
                          type="button"
                          onClick={() => dispatch(setAlbumCover({ albumId: id, imageId: img.id }))}
                          disabled={isCover}
                          className="uppercase tracking-[0.18em] text-ink/70 hover:text-ink disabled:text-brand disabled:cursor-default"
                        >
                          {isCover ? '★' : t('admin.cover')}
                        </button>
                        <button
                          type="button"
                          onClick={() => {
                            if (window.confirm(t('admin.confirmDelete'))) {
                              dispatch(deleteAlbumImage({ albumId: id, imageId: img.id }));
                            }
                          }}
                          className="uppercase tracking-[0.18em] text-red-600 hover:text-red-700"
                        >
                          {t('admin.delete')}
                        </button>
                      </div>
                    </div>
                  </li>
                );
              })}
            </ul>
          )}
        </section>
      )}
    </div>
  );
}

const inputCls =
  'mt-2 w-full rounded-lg border border-ink/20 bg-paper px-3 py-2 text-ink focus:border-ink focus:outline-none';

function Field({ label, children, className }: { label: string; children: React.ReactNode; className?: string }): React.JSX.Element {
  return (
    <label className={['block text-xs uppercase tracking-[0.2em] text-ink/70', className].filter(Boolean).join(' ')}>
      {label}
      {children}
    </label>
  );
}

function Checkbox({ label, checked, onChange }: { label: string; checked: boolean; onChange: (v: boolean) => void }): React.JSX.Element {
  return (
    <label className="inline-flex items-center gap-2 text-sm text-ink">
      <input type="checkbox" checked={checked} onChange={(e) => onChange(e.target.checked)} className="h-4 w-4" />
      {label}
    </label>
  );
}
