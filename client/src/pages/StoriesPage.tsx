import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { SectionHeading } from '@/shared/ui/SectionHeading';
import { AlbumsContainer } from '@/features/albums/ui/AlbumsContainer';

export function StoriesPage(): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <section className="mx-auto w-full max-w-6xl px-6 py-12 sm:py-16">
      <SectionHeading title={t('stories.title')} subtitle={t('stories.subtitle')} />
      <div className="mt-10">
        <AlbumsContainer filter="stories" />
      </div>
    </section>
  );
}
