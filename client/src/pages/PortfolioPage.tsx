import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { AlbumsContainer } from '@/features/albums/ui/AlbumsContainer';

export function PortfolioPage(): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <main className="mx-auto max-w-6xl px-6 py-12">
      <h1 className="mb-8 text-4xl font-bold leading-[1.1] tracking-tight sm:text-5xl">
        {t('nav.portfolio')}
      </h1>
      <AlbumsContainer />
    </main>
  );
}
