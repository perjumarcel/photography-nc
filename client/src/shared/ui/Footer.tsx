import * as React from 'react';
import { useTranslation } from 'react-i18next';

/**
 * Site footer.
 *
 * Mirrors the legacy `_Footer.cshtml` block: copyright + contact links on the
 * left, social icons on the right. Stacks on mobile, splits into two columns
 * on `sm` and up.
 */
export function Footer(): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <footer className="border-t border-rule bg-paper">
      <div className="mx-auto flex w-full max-w-6xl flex-col gap-6 px-6 py-8 sm:flex-row sm:items-center sm:justify-between">
        <div className="space-y-2 text-center sm:text-left">
          <p className="text-xs uppercase tracking-[0.2em] text-ink-muted">
            {t('footer.rights', { year: new Date().getFullYear() })}
          </p>
          <ul className="flex flex-wrap justify-center gap-x-4 gap-y-1 text-sm text-ink sm:justify-start">
            <li>
              <a className="hover:text-brand" href="mailto:NickCovercenco@yahoo.com">
                nickcovercenco@yahoo.com
              </a>
            </li>
            <li>
              <a className="hover:text-brand" href="tel:+37368538087">
                +373 (68) 538 087
              </a>
            </li>
          </ul>
        </div>

        <ul className="flex justify-center gap-3 sm:justify-end">
          <li>
            <a
              aria-label={t('footer.followFacebook')}
              href="https://www.facebook.com/NicolaeCovercencoPhotography/"
              target="_blank"
              rel="noreferrer noopener"
              className="inline-flex h-10 w-10 items-center justify-center rounded-full border border-rule text-ink hover:border-ink hover:text-brand"
            >
              <IconFacebook />
            </a>
          </li>
          <li>
            <a
              aria-label={t('footer.followInstagram')}
              href="https://www.instagram.com/nicolae_covercenco/"
              target="_blank"
              rel="noreferrer noopener"
              className="inline-flex h-10 w-10 items-center justify-center rounded-full border border-rule text-ink hover:border-ink hover:text-brand"
            >
              <IconInstagram />
            </a>
          </li>
          <li>
            <a
              aria-label={t('footer.followTumblr')}
              href="https://www.tumblr.com/"
              target="_blank"
              rel="noreferrer noopener"
              className="inline-flex h-10 w-10 items-center justify-center rounded-full border border-rule text-ink hover:border-ink hover:text-brand"
            >
              <IconTumblr />
            </a>
          </li>
        </ul>
      </div>
    </footer>
  );
}

function IconFacebook(): React.JSX.Element {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
      <path d="M13.5 21v-7h2.4l.4-3h-2.8V9c0-.9.3-1.5 1.6-1.5h1.7V4.9c-.3 0-1.3-.1-2.4-.1-2.4 0-4 1.5-4 4.1V11H8v3h2.4v7h3.1z" />
    </svg>
  );
}

function IconInstagram(): React.JSX.Element {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" aria-hidden="true">
      <rect x="3" y="3" width="18" height="18" rx="5" />
      <circle cx="12" cy="12" r="4" />
      <circle cx="17.5" cy="6.5" r="0.9" fill="currentColor" />
    </svg>
  );
}

function IconTumblr(): React.JSX.Element {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
      <path d="M14 3v4h3v3h-3v5c0 1 .3 1.5 1.5 1.5H17V20c-.6.2-1.6.4-2.5.4-2.7 0-3.5-1.7-3.5-3.6V10H9V7.4c2-.7 2.6-2.4 2.7-4.4H14z" />
    </svg>
  );
}
