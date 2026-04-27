import * as React from 'react';
import { NavLink } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { cn } from '@/shared/lib/cn';

interface NavItem {
  to: string;
  labelKey: string;
}

const items: NavItem[] = [
  { to: '/', labelKey: 'nav.home' },
  { to: '/portfolio', labelKey: 'nav.portfolio' },
  { to: '/stories', labelKey: 'nav.stories' },
  { to: '/about', labelKey: 'nav.about' },
  { to: '/contact', labelKey: 'nav.contact' },
];

/**
 * Site header / sidebar.
 *
 * Layout:
 *  - On mobile (< lg): a fixed top bar with a brand wordmark on the left and a
 *    hamburger button on the right. Tapping the hamburger reveals an overlay
 *    drawer with the navigation items.
 *  - On desktop (≥ lg): a fixed full-height left sidebar containing the brand,
 *    nav and language switcher — preserving the legacy "outdoor" layout.
 */
export function Header(): React.JSX.Element {
  const { t, i18n } = useTranslation();
  const [open, setOpen] = React.useState(false);

  // Close the mobile drawer with Escape and on route change.
  React.useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent): void => {
      if (e.key === 'Escape') setOpen(false);
    };
    document.addEventListener('keydown', onKey);
    document.body.style.overflow = 'hidden';
    return () => {
      document.removeEventListener('keydown', onKey);
      document.body.style.overflow = '';
    };
  }, [open]);

  const switchLang = (): void => {
    void i18n.changeLanguage(i18n.resolvedLanguage === 'ro' ? 'en' : 'ro');
  };

  return (
    <>
      {/* Mobile top bar */}
      <div className="fixed inset-x-0 top-0 z-40 flex items-center justify-between bg-shell/95 px-4 py-3 backdrop-blur lg:hidden">
        <Brand />
        <button
          type="button"
          aria-expanded={open}
          aria-controls="primary-navigation"
          aria-label={open ? t('nav.closeMenu') : t('nav.openMenu')}
          onClick={() => setOpen((v) => !v)}
          className="inline-flex h-10 w-10 items-center justify-center text-paper hover:text-brand"
        >
          {open ? <IconClose /> : <IconMenu />}
        </button>
      </div>

      {/* Backdrop for mobile drawer */}
      {open && (
        <button
          aria-hidden="true"
          tabIndex={-1}
          className="fixed inset-0 z-40 bg-black/50 lg:hidden"
          onClick={() => setOpen(false)}
        />
      )}

      {/* Sidebar / drawer */}
      <aside
        id="primary-navigation"
        className={cn(
          'fixed inset-y-0 left-0 z-50 flex w-[var(--shell-w)] flex-col justify-between bg-shell text-paper',
          'transition-transform duration-300 ease-out',
          open ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
        )}
      >
        <div className="flex flex-col gap-10 px-8 pb-8 pt-10">
          <Brand large />
          <nav aria-label="Primary">
            <ul className="space-y-2">
              {items.map((item) => (
                <li key={item.to}>
                  <NavLink
                    to={item.to}
                    end={item.to === '/'}
                    onClick={() => setOpen(false)}
                    className={({ isActive }) =>
                      cn(
                        'block py-2 text-[0.8rem] uppercase tracking-[0.25em] transition-colors',
                        isActive ? 'text-brand' : 'text-paper/80 hover:text-paper',
                      )
                    }
                  >
                    {t(item.labelKey)}
                  </NavLink>
                </li>
              ))}
            </ul>
          </nav>
        </div>
        <div className="flex items-center justify-between gap-3 border-t border-white/10 px-8 py-6 text-xs uppercase tracking-[0.25em]">
          <button
            type="button"
            onClick={switchLang}
            aria-label={t('nav.languageSwitch')}
            className="text-paper/70 hover:text-brand"
          >
            {i18n.resolvedLanguage === 'ro' ? 'EN' : 'RO'}
          </button>
          <span className="text-paper/40">© {new Date().getFullYear()}</span>
        </div>
      </aside>
    </>
  );
}

function Brand({ large = false }: { large?: boolean }): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <NavLink to="/" className="group inline-flex flex-col leading-none">
      <span
        className={cn(
          'font-display text-paper',
          large ? 'text-3xl' : 'text-xl',
        )}
      >
        {t('app.brand')}
      </span>
      <span className="mt-1 text-[0.65rem] uppercase tracking-[0.4em] text-paper/60 group-hover:text-brand">
        {t('app.tagline')}
      </span>
    </NavLink>
  );
}

function IconMenu(): React.JSX.Element {
  return (
    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <path d="M3 6h18M3 12h18M3 18h18" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
    </svg>
  );
}

function IconClose(): React.JSX.Element {
  return (
    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <path d="M6 6l12 12M18 6L6 18" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
    </svg>
  );
}
