import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { SectionHeading } from '@/shared/ui/SectionHeading';
import { AlbumsContainer } from '@/features/albums/ui/AlbumsContainer';
import { Seo } from '@/shared/ui/Seo';

export function PortfolioPage(): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <section className="mx-auto w-full max-w-6xl px-6 py-12 sm:py-16">
      <Seo title={`${t('portfolio.title')} — ${t('app.title')}`} description={t('portfolio.subtitle')} canonicalPath="/portfolio" />
      <SectionHeading title={t('portfolio.title')} subtitle={t('portfolio.subtitle')} />
      <div className="mt-10">
        <AlbumsContainer filter="portfolio" />
      </div>
    </section>
  );
}
