import * as React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { ResponsiveImage } from '@/shared/ui/ResponsiveImage';
import { Seo } from '@/shared/ui/Seo';
import type { AlbumDetailsDto } from '../model/types';
import { PhotoGallery } from './PhotoGallery';

interface AlbumDetailProps {
  album: AlbumDetailsDto;
  categoryName?: string;
  previousAlbum?: { id: string; title: string };
  nextAlbum?: { id: string; title: string };
}

/**
 * Album detail. Mirrors the legacy `Portfolio/Details.cshtml` layout:
 *  - Hero with the cover image as background, album title overlaid.
 *  - Two-column "info" panel on desktop, stacked on mobile.
 *  - Image grid that scales from 1 → 2 → 3 columns and varies row span by
 *    image orientation so portraits stay portrait without forced cropping.
 */
export function AlbumDetail({ album, categoryName, previousAlbum, nextAlbum }: AlbumDetailProps): React.JSX.Element {
  const { t, i18n } = useTranslation();
  const cover = album.images.find((i) => i.imageType === 1) ?? album.images[0];
  const seoDescription = album.description ?? [categoryName, album.location].filter(Boolean).join(' · ');

  return (
    <article>
      <Seo
        title={`${album.title} — ${t('app.title')}`}
        description={seoDescription || t('portfolio.subtitle')}
        image={album.coverVariants?.hero ?? cover?.variants.hero ?? cover?.publicUrl}
        canonicalPath={`/portfolio/${album.id}`}
      />
      {/* Hero / parallax-style cover */}
      <section
        className="relative flex h-[60vh] min-h-[360px] w-full items-end overflow-hidden bg-ink-soft text-paper"
        aria-labelledby="album-title"
      >
        {cover && (
          <ResponsiveImage
            src={cover.publicUrl}
            variants={cover.variants}
            alt=""
            width={cover.width}
            height={cover.height}
            sizes="100vw"
            className="absolute inset-0 h-full w-full"
            imgClassName="opacity-70"
            loading="eager"
            fetchPriority="high"
          />
        )}
        <div aria-hidden="true" className="absolute inset-0 bg-gradient-to-t from-ink/80 via-ink/30 to-transparent" />
        <div className="relative mx-auto w-full max-w-6xl px-6 pb-10 sm:pb-16">
          <p className="text-[0.7rem] uppercase tracking-[0.4em] text-paper/70">
            {t('portfolio.title')}
          </p>
          <h1
            id="album-title"
            className="mt-2 font-display text-3xl font-light leading-tight sm:text-5xl md:text-6xl"
          >
            {album.title}
          </h1>
        </div>
      </section>

      {/* Info + gallery */}
      <section className="mx-auto w-full max-w-6xl px-6 py-12">
        <div className="grid gap-10 md:grid-cols-3">
          <aside className="space-y-4 md:col-span-1">
            <h2 className="font-display text-xl text-ink">{t('album.info')}</h2>
            <span aria-hidden="true" className="legacy-separator" />
            {album.description && (
              <p className="whitespace-pre-line text-sm leading-relaxed text-ink-muted">
                {album.description}
              </p>
            )}
            <dl className="space-y-3 text-sm">
              {categoryName && (
                <InfoRow label={t('album.category')}>{categoryName}</InfoRow>
              )}
              {album.eventDate && (
                <InfoRow label={t('album.date')}>{formatDate(album.eventDate, i18n.resolvedLanguage)}</InfoRow>
              )}
              {album.client && <InfoRow label={t('album.client')}>{album.client}</InfoRow>}
              {album.location && (
                <InfoRow label={t('album.location')}>{album.location}</InfoRow>
              )}
            </dl>

            <div className="pt-4">
              <div className="flex flex-wrap gap-4 text-xs uppercase tracking-[0.25em] text-ink">
                {previousAlbum && (
                  <Link to={`/portfolio/${previousAlbum.id}`} className="underline-offset-4 hover:underline">
                    ← {t('common.previous')}
                  </Link>
                )}
                <Link to="/portfolio" className="underline-offset-4 hover:underline">
                  {t('common.backToList')}
                </Link>
                {nextAlbum && (
                  <Link to={`/portfolio/${nextAlbum.id}`} className="underline-offset-4 hover:underline">
                    {t('common.next')} →
                  </Link>
                )}
              </div>
            </div>
          </aside>

          <div className="md:col-span-2">
            {album.images.length === 0 ? (
              <p className="text-ink-muted">{t('common.empty')}</p>
            ) : (
              <PhotoGallery images={album.images} />
            )}
          </div>
        </div>
        {(previousAlbum || nextAlbum) && (
          <nav aria-label={t('album.navigation')} className="mt-12 flex justify-between gap-4 border-t border-rule pt-6 text-sm">
            {previousAlbum ? (
              <Link to={`/portfolio/${previousAlbum.id}`} className="max-w-[45%] text-ink hover:text-brand">
                <span className="block text-[0.65rem] uppercase tracking-[0.25em] text-ink-muted">{t('common.previous')}</span>
                <span className="mt-1 block truncate">← {previousAlbum.title}</span>
              </Link>
            ) : <span />}
            {nextAlbum ? (
              <Link to={`/portfolio/${nextAlbum.id}`} className="max-w-[45%] text-right text-ink hover:text-brand">
                <span className="block text-[0.65rem] uppercase tracking-[0.25em] text-ink-muted">{t('common.next')}</span>
                <span className="mt-1 block truncate">{nextAlbum.title} →</span>
              </Link>
            ) : <span />}
          </nav>
        )}
      </section>
    </article>
  );
}

function InfoRow({ label, children }: { label: string; children: React.ReactNode }): React.JSX.Element {
  return (
    <div className="flex items-start gap-3">
      <dt className="w-20 shrink-0 text-[0.7rem] uppercase tracking-[0.25em] text-ink-muted">
        {label}
      </dt>
      <dd className="text-ink">{children}</dd>
    </div>
  );
}

function formatDate(iso: string, locale: string | undefined): string {
  try {
    return new Date(iso).toLocaleDateString(locale ?? 'ro', {
      year: 'numeric',
      month: 'long',
    });
  } catch {
    return iso;
  }
}
