import * as React from 'react';
import { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { fetchPublicAlbums } from '@/features/albums/api/thunks';
import { Button } from '@/shared/ui/Button';
import { Skeleton } from '@/shared/ui/Skeleton';

/**
 * Home page. Mirrors the legacy `Home/Index.cshtml`:
 *  - Hero with a tagline and CTA.
 *  - Horizontal carousel of "home" albums beneath. On mobile this becomes a
 *    horizontally-scrollable strip; on desktop it uses snap scrolling so users
 *    can swipe through covers without a full re-layout.
 */
export function HomePage(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const { list, listStatus } = useAppSelector((s) => s.albums);

  useEffect(() => {
    if (listStatus === 'idle') void dispatch(fetchPublicAlbums());
  }, [dispatch, listStatus]);

  const featured = list.filter((a) => a.showInHome).slice(0, 12);

  return (
    <>
      {/* Hero */}
      <section className="mx-auto flex w-full max-w-6xl flex-col justify-center px-6 py-16 sm:py-20 lg:py-28">
        <p className="text-[0.7rem] uppercase tracking-[0.4em] text-ink-muted">
          {t('home.kicker')}
        </p>
        <h1 className="mt-4 max-w-3xl font-display text-4xl font-light leading-[1.05] text-ink sm:text-5xl md:text-6xl lg:text-7xl">
          {t('home.title')}
        </h1>
        <span aria-hidden="true" className="legacy-separator" />
        <p className="mt-2 max-w-2xl text-base text-ink-muted sm:text-lg">
          {t('home.subtitle')}
        </p>
        <div className="mt-8">
          <Link to="/portfolio">
            <Button variant="primary">{t('home.cta')}</Button>
          </Link>
        </div>
      </section>

      {/* Featured carousel */}
      <section
        aria-label={t('portfolio.title')}
        className="mx-auto w-full max-w-7xl px-6 pb-20"
      >
        {listStatus === 'loading' || listStatus === 'idle' ? (
          <div className="flex gap-6 overflow-hidden">
            {Array.from({ length: 4 }).map((_, i) => (
              <Skeleton key={i} className="h-[60vh] min-h-[320px] w-[80vw] flex-shrink-0 sm:w-[55vw] md:w-[40vw] lg:w-[30vw]" />
            ))}
          </div>
        ) : featured.length === 0 ? (
          <p className="text-center text-ink-muted">{t('common.empty')}</p>
        ) : (
          <ul className="-mx-6 flex snap-x snap-mandatory gap-6 overflow-x-auto px-6 pb-4">
            {featured.map((album) => (
              <li
                key={album.id}
                className="snap-start"
              >
                <Link
                  to={`/portfolio/${album.id}`}
                  className="group relative block h-[60vh] min-h-[320px] w-[80vw] overflow-hidden bg-paper-soft sm:w-[55vw] md:w-[40vw] lg:w-[30vw]"
                  aria-label={album.title}
                >
                  <div className="absolute inset-0 flex items-center justify-center text-ink-muted">
                    <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.2" aria-hidden="true">
                      <rect x="3" y="3" width="18" height="18" rx="2" />
                    </svg>
                  </div>
                  <span aria-hidden="true" className="absolute inset-0 bg-ink/20 transition-colors group-hover:bg-ink/40" />
                  <div className="absolute bottom-0 left-0 right-0 p-5 text-paper">
                    <h3 className="font-display text-2xl">{album.title}</h3>
                    {album.location && (
                      <p className="mt-1 text-[0.7rem] uppercase tracking-[0.25em] text-paper/80">
                        {album.location}
                      </p>
                    )}
                  </div>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>
    </>
  );
}
