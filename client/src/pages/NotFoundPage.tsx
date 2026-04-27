import * as React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export function NotFoundPage(): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <section className="mx-auto flex w-full max-w-3xl flex-col items-center px-6 py-24 text-center">
      <p className="text-[0.7rem] uppercase tracking-[0.4em] text-ink-muted">404</p>
      <h1 className="mt-3 font-display text-4xl font-light text-ink sm:text-5xl">
        {t('common.empty')}
      </h1>
      <span aria-hidden="true" className="legacy-separator mx-auto" />
      <Link
        to="/"
        className="mt-2 text-xs uppercase tracking-[0.25em] text-ink underline-offset-4 hover:underline"
      >
        ← {t('nav.home')}
      </Link>
    </section>
  );
}
