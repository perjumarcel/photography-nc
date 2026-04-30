import * as React from 'react';
import { useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { fetchAlbumById, fetchPublicAlbums } from '../api/thunks';
import { fetchPublicCategories } from '@/features/categories/api/thunks';
import { clearCurrent } from '../model/albumsSlice';
import { AlbumDetail } from './AlbumDetail';

/** Container — wires the URL param to the slice and forwards to the presenter. */
export function AlbumDetailContainer(): React.JSX.Element {
  const { id = '' } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const { current, currentStatus, currentError } = useAppSelector((s) => s.albums);
  const { list, listStatus } = useAppSelector((s) => s.albums);
  const categories = useAppSelector((s) => s.categories.list);
  const { t } = useTranslation();

  useEffect(() => {
    if (id) void dispatch(fetchAlbumById(id));
    if (listStatus === 'idle') void dispatch(fetchPublicAlbums());
    if (categories.length === 0) void dispatch(fetchPublicCategories());
    return () => {
      dispatch(clearCurrent());
    };
  }, [dispatch, id, categories.length, listStatus]);

  if (currentStatus === 'loading' || currentStatus === 'idle') {
    return (
      <div className="px-6 py-20 text-center text-ink-muted">
        <p aria-busy="true">{t('common.loading')}</p>
      </div>
    );
  }

  if (currentStatus === 'failed' || !current) {
    return (
      <div className="mx-auto max-w-2xl px-6 py-20 text-center">
        <p role="alert" className="text-red-700">
          {currentError ?? t('common.error')}
        </p>
        <Link
          to="/portfolio"
          className="mt-6 inline-block text-xs uppercase tracking-[0.25em] text-ink underline-offset-4 hover:underline"
        >
          {t('common.backToList')}
        </Link>
      </div>
    );
  }

  const categoryName = categories.find((c) => c.id === current.categoryId)?.name;
  const currentIndex = list.findIndex((album) => album.id === current.id);
  const previousAlbum = currentIndex > 0 ? list[currentIndex - 1] : undefined;
  const nextAlbum = currentIndex >= 0 && currentIndex < list.length - 1 ? list[currentIndex + 1] : undefined;

  return (
    <AlbumDetail
      album={current}
      categoryName={categoryName}
      previousAlbum={previousAlbum && { id: previousAlbum.slug || previousAlbum.id, title: previousAlbum.title }}
      nextAlbum={nextAlbum && { id: nextAlbum.slug || nextAlbum.id, title: nextAlbum.title }}
    />
  );
}
