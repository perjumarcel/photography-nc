import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { Outlet } from 'react-router-dom';
import { Header } from './Header';
import { Footer } from './Footer';

/**
 * Top-level chrome. Mirrors the legacy `_Layout.cshtml`:
 *  - Sidebar on the left (Header).
 *  - Main scrollable content with the page body.
 *  - Footer at the bottom.
 *  - Decorative side strips on very wide screens (legacy `.left-decor` /
 *    `.right-decor`), purely aesthetic; collapsed on small viewports.
 */
export function Layout(): React.JSX.Element {
  const { t } = useTranslation();

  return (
    <div className="min-h-screen bg-paper text-ink">
      <a className="skip-link" href="#main">
        {t('nav.skipToContent')}
      </a>

      <Header />

      {/* Decorative strips — visible only on very wide screens, behind content. */}
      <div
        aria-hidden="true"
        className="pointer-events-none fixed inset-y-0 right-0 z-0 hidden w-[var(--decor-w)] bg-paper-soft xl:block"
      />

      <div className="lg:pl-[var(--shell-w)]">
        <main id="main" className="min-h-[calc(100vh-200px)] pt-16 lg:pt-0">
          <Outlet />
        </main>
        <Footer />
      </div>
    </div>
  );
}
