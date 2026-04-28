import * as React from 'react';
import { Skeleton } from '@/shared/ui/Skeleton';
import { testIds } from '@/shared/lib/testIds';
import { AlbumCard } from './AlbumCard';
import type { AlbumDto, LoadStatus } from '../model/types';

interface AlbumsListProps {
  albums: AlbumDto[];
  status: LoadStatus;
  error: string | null;
  /** Map of category id → display name, used to label cards. */
  categoryNames?: Record<number, string>;
  /** Map of album id → cover URL, supplied by the container. */
  coverUrls?: Record<string, string | undefined>;
  labels: { empty: string; loading: string; error: string };
}

/**
 * Pure presenter — receives all data via props, no Redux hooks.
 * Renders a responsive 1/2/3-column grid that matches the legacy
 * `.gallery-items.three-coulms` layout but uses CSS grid for cleaner
 * responsive behaviour.
 */
export function AlbumsList({
  albums,
  status,
  error,
  categoryNames,
  coverUrls,
  labels,
}: AlbumsListProps): React.JSX.Element {
  if (status === 'loading' || status === 'idle') {
    return (
      <div
        data-testid={testIds.album.list}
        aria-busy="true"
        className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3"
      >
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="space-y-3">
            <Skeleton className="aspect-[4/5] w-full sm:aspect-[4/3]" />
            <Skeleton className="h-4 w-2/3" />
            <Skeleton className="h-3 w-1/3" />
          </div>
        ))}
        <span className="sr-only">{labels.loading}</span>
      </div>
    );
  }
  if (status === 'failed') {
    return (
      <div
        role="alert"
        className="rounded-sm border border-red-200 bg-red-50 p-5 text-red-800"
      >
        {error ?? labels.error}
      </div>
    );
  }
  if (albums.length === 0) {
    return (
      <div className="border border-dashed border-rule bg-paper-soft p-10 text-center text-ink-muted">
        {labels.empty}
      </div>
    );
  }
  return (
    <ul
      data-testid={testIds.album.list}
      className="grid list-none gap-6 p-0 sm:grid-cols-2 lg:grid-cols-3"
    >
      {albums.map((album) => (
        <li key={album.id}>
          <AlbumCard
            album={album}
            coverUrl={coverUrls?.[album.id]}
            categoryName={categoryNames?.[album.categoryId]}
          />
        </li>
      ))}
    </ul>
  );
}
