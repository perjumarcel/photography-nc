import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { Seo } from '@/shared/ui/Seo';

/**
 * About page. Mirrors `About/Index.cshtml` — a full-bleed image and a text
 * column. Stacks vertically on mobile, switches to side-by-side on `md+`.
 */
export function AboutPage(): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <section className="bg-ink-soft text-paper">
      <Seo title={`${t('about.title')} — ${t('app.title')}`} description={t('about.subtitle')} canonicalPath="/about" />
      <div className="mx-auto grid w-full max-w-6xl gap-10 px-6 py-12 sm:py-16 md:grid-cols-2 md:items-center md:gap-16 md:py-24">
        <figure className="relative aspect-[4/5] w-full overflow-hidden bg-shell md:aspect-[3/4]">
          {/* The legacy image lived at /images/about-me.jpg. Once the asset is
              migrated to R2 it can be referenced here directly. */}
          <div className="flex h-full w-full items-center justify-center text-paper/40">
            <svg width="80" height="80" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" aria-hidden="true">
              <circle cx="12" cy="9" r="3.5" />
              <path d="M4 21c1.6-4 4.6-6 8-6s6.4 2 8 6" />
            </svg>
          </div>
        </figure>

        <div>
          <p className="text-[0.7rem] uppercase tracking-[0.4em] text-paper/60">
            {t('app.brand')}
          </p>
          <h1 className="mt-3 font-display text-3xl font-light leading-tight text-paper sm:text-4xl md:text-5xl">
            {t('about.title')}
          </h1>
          <span aria-hidden="true" className="legacy-separator !bg-paper" />
          <p className="mt-2 text-base text-paper/80 sm:text-lg">{t('about.subtitle')}</p>
          <p className="mt-6 leading-relaxed text-paper/70">{t('about.body')}</p>
        </div>
      </div>
    </section>
  );
}
