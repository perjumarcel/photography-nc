import * as React from 'react';
import { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { fetchPublicAlbums } from '../api/thunks';
import { fetchPublicCategories } from '@/features/categories/api/thunks';
import { AlbumsList } from './AlbumsList';
import { CategoryFilters, type CategoryFilterOption } from './CategoryFilters';

interface AlbumsContainerProps {
  /** Optional client-side filter on the album list (e.g. only home/stories albums). */
  filter?: 'portfolio' | 'stories' | 'home' | 'all';
}

export function AlbumsContainer({ filter = 'portfolio' }: AlbumsContainerProps): React.JSX.Element {
  const dispatch = useAppDispatch();
  const { list, listStatus, listError } = useAppSelector((s) => s.albums);
  const { list: categories, status: catStatus } = useAppSelector((s) => s.categories);
  const { t } = useTranslation();
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

  useEffect(() => {
    if (listStatus === 'idle') void dispatch(fetchPublicAlbums());
    if (catStatus === 'idle') void dispatch(fetchPublicCategories());
  }, [dispatch, listStatus, catStatus]);

  const visibleAlbums = useMemo(() => {
    let albums = list;
    switch (filter) {
      case 'home':
        albums = albums.filter((a) => a.showInHome);
        break;
      case 'stories':
        albums = albums.filter((a) => a.showInStories);
        break;
      case 'portfolio':
        albums = albums.filter((a) => a.showInPortfolio);
        break;
    }
    if (selectedCategoryId !== null) {
      albums = albums.filter((a) => a.categoryId === selectedCategoryId);
    }
    return albums;
  }, [list, filter, selectedCategoryId]);

  const filterOptions: CategoryFilterOption[] = useMemo(
    () => [
      { id: null, label: t('common.allAlbums') },
      ...categories.map((c) => ({ id: c.id, label: c.name })),
    ],
    [categories, t],
  );

  const categoryNames = useMemo<Record<number, string>>(
    () => Object.fromEntries(categories.map((c) => [c.id, c.name])),
    [categories],
  );

  return (
    <>
      {filterOptions.length > 1 && (
        <CategoryFilters
          options={filterOptions}
          selectedId={selectedCategoryId}
          onSelect={setSelectedCategoryId}
        />
      )}
      <AlbumsList
        albums={visibleAlbums}
        status={listStatus}
        error={listError}
        categoryNames={categoryNames}
        labels={{
          empty: t('album.empty'),
          loading: t('common.loading'),
          error: t('album.error'),
        }}
      />
    </>
  );
}
