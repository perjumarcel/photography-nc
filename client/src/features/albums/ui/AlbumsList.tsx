import * as React from 'react';
import { Skeleton } from '@/shared/ui/Skeleton';
import { testIds } from '@/shared/lib/testIds';
import type { AlbumDto, LoadStatus } from '../model/types';

interface AlbumsListProps {
  albums: AlbumDto[];
  status: LoadStatus;
  error: string | null;
  labels: { empty: string; loading: string; error: string };
}

/** Pure presenter — receives all data via props, no Redux hooks. */
export function AlbumsList({ albums, status, error, labels }: AlbumsListProps): React.JSX.Element {
  if (status === 'loading' || status === 'idle') {
    return (
      <div data-testid={testIds.album.list} aria-busy="true" className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {Array.from({ length: 6 }).map((_, i) => (
          <Skeleton key={i} className="aspect-[4/3] w-full" />
        ))}
        <span className="sr-only">{labels.loading}</span>
      </div>
    );
  }
  if (status === 'failed') {
    return (
      <div role="alert" className="rounded-xl border border-red-500/30 bg-red-500/10 p-5 text-red-200">
        {error ?? labels.error}
      </div>
    );
  }
  if (albums.length === 0) {
    return (
      <div className="rounded-xl border border-zinc-700 bg-surface-800 p-8 text-center text-zinc-300">
        {labels.empty}
      </div>
    );
  }
  return (
    <ul data-testid={testIds.album.list} className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {albums.map((album) => (
        <li
          key={album.id}
          data-testid={testIds.album.card}
          className="rounded-xl border border-zinc-700 bg-surface-800 p-5 transition-colors hover:border-brand/40"
        >
          <h3 className="text-xl font-bold">{album.title}</h3>
          {album.location && <p className="mt-1 text-sm text-zinc-400">{album.location}</p>}
          <p className="mt-3 text-sm text-zinc-500">{album.imageCount} images</p>
        </li>
      ))}
    </ul>
  );
}
