import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { ContactFormContainer } from '@/features/contact/ui/ContactFormContainer';
import { Seo } from '@/shared/ui/Seo';

/**
 * Contact page. Mirrors `Contact/Index.cshtml` — info column on the left,
 * contact form on the right. Stacks on mobile. The form posts to
 * `/api/public/contact` via the {@link ContactFormContainer}.
 */
export function ContactPage(): React.JSX.Element {
  const { t } = useTranslation();

  return (
    <section className="bg-ink-soft text-paper">
      <Seo title={`${t('contact.title')} — ${t('app.title')}`} description={t('contact.intro1')} canonicalPath="/contact" />
      <div className="mx-auto w-full max-w-6xl px-6 py-12 sm:py-16">
        <p className="text-[0.7rem] uppercase tracking-[0.4em] text-paper/60">
          {t('app.brand')}
        </p>
        <h1 className="mt-3 font-display text-3xl font-light leading-tight text-paper sm:text-4xl md:text-5xl">
          {t('contact.title')}
        </h1>
        <span aria-hidden="true" className="legacy-separator !bg-paper" />

        <div className="mt-10 grid gap-12 md:grid-cols-3">
          {/* Info */}
          <div className="space-y-4 text-paper/80 md:col-span-1">
            <p>{t('contact.intro1')}</p>
            <p>{t('contact.intro2')}</p>
            <ul className="mt-6 space-y-2">
              <li>
                <a
                  href="https://maps.google.com/?q=Strada+Calea+Iesilor+10+Chisinau"
                  target="_blank"
                  rel="noreferrer noopener"
                  className="text-paper hover:text-brand"
                >
                  {t('contact.address')}
                </a>
              </li>
              <li>
                <a className="text-paper hover:text-brand" href="tel:+37368538087">
                  {t('contact.phone')}
                </a>
              </li>
              <li>
                <a className="text-paper hover:text-brand" href="mailto:NickCovercenco@yahoo.com">
                  {t('contact.email')}
                </a>
              </li>
            </ul>
          </div>

          <ContactFormContainer />
        </div>
      </div>
    </section>
  );
}
