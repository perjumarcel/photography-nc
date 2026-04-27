import * as React from 'react';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { fetchPublicAlbums } from '../api/thunks';
import { AlbumsList } from './AlbumsList';

export function AlbumsContainer(): React.JSX.Element {
  const dispatch = useAppDispatch();
  const { list, listStatus, listError } = useAppSelector((s) => s.albums);
  const { t } = useTranslation();

  useEffect(() => {
    if (listStatus === 'idle') void dispatch(fetchPublicAlbums());
  }, [dispatch, listStatus]);

  return (
    <AlbumsList
      albums={list}
      status={listStatus}
      error={listError}
      labels={{
        empty: t('album.empty'),
        loading: t('album.loading'),
        error: t('album.error'),
      }}
    />
  );
}
