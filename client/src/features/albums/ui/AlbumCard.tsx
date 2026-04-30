import * as React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { testIds } from '@/shared/lib/testIds';
import { ResponsiveImage } from '@/shared/ui/ResponsiveImage';
import type { AlbumDto } from '../model/types';

interface AlbumCardProps {
  album: AlbumDto;
  /** Display name of the album's category, when known. */
  categoryName?: string;
}

/**
 * Album card. Mirrors the legacy `.gallery-item` block — image with a dark
 * overlay on hover, title and category beneath. Aspect-ratio is kept fixed so
 * the grid stays even regardless of cover dimensions.
 */
export function AlbumCard({ album, categoryName }: AlbumCardProps): React.JSX.Element {
  const { t } = useTranslation();
  const href = `/portfolio/${album.id}`;

  return (
    <article
      data-testid={testIds.album.card}
      className="group flex flex-col gap-3"
    >
      <Link
        to={href}
        aria-label={t('common.viewAlbum') + ': ' + album.title}
        className="relative block aspect-[4/5] overflow-hidden bg-paper-soft sm:aspect-[4/3]"
      >
        {album.coverPublicUrl ? (
          <ResponsiveImage
            src={album.coverPublicUrl}
            variants={album.coverVariants}
            alt={album.title}
            width={album.coverWidth}
            height={album.coverHeight}
            sizes="(min-width: 1024px) 33vw, (min-width: 640px) 50vw, 100vw"
            className="h-full w-full"
            imgClassName="transition-transform duration-700 ease-out group-hover:scale-[1.04] motion-reduce:transition-none motion-reduce:transform-none"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-ink-muted">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" aria-hidden="true">
              <rect x="3" y="3" width="18" height="18" rx="2" />
              <circle cx="9" cy="9" r="2" />
              <path d="M21 15l-5-5L5 21" />
            </svg>
          </div>
        )}
        <span
          aria-hidden="true"
          className="absolute inset-0 bg-ink/0 transition-colors duration-300 group-hover:bg-ink/30"
        />
      </Link>

      <div className="px-1">
        <h3 className="font-display text-lg leading-tight text-ink">
          <Link to={href} className="hover:text-brand">
            {album.title}
          </Link>
        </h3>
        {categoryName && (
          <p className="mt-1 text-[0.7rem] uppercase tracking-[0.25em] text-ink-muted">
            {categoryName}
          </p>
        )}
      </div>
    </article>
  );
}
