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
  const directLinks = [
    { label: t('contact.actionCall'), href: 'tel:+37368538087' },
    { label: t('contact.actionWhatsApp'), href: 'https://wa.me/37368538087' },
    { label: t('contact.actionEmail'), href: 'mailto:NickCovercenco@yahoo.com' },
    { label: t('contact.actionInstagram'), href: 'https://www.instagram.com/nicolae_covercenco/' },
  ];

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
          <div className="space-y-6 text-paper/80 md:col-span-1">
            <p>{t('contact.intro1')}</p>
            <p>{t('contact.intro2')}</p>
            <ul className="space-y-2">
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
            <div>
              <p className="text-[0.7rem] uppercase tracking-[0.25em] text-paper/60">
                {t('contact.directTitle')}
              </p>
              <div className="mt-3 flex flex-wrap gap-3">
                {directLinks.map((link) => (
                  <a
                    key={link.href}
                    href={link.href}
                    target={link.href.startsWith('http') ? '_blank' : undefined}
                    rel={link.href.startsWith('http') ? 'noreferrer noopener' : undefined}
                    className="rounded-full border border-paper/25 px-4 py-2 text-sm text-paper transition hover:border-brand hover:text-brand"
                  >
                    {link.label}
                  </a>
                ))}
              </div>
            </div>
            <div className="space-y-2 border-l border-brand/60 pl-4 text-sm text-paper/70">
              <p>{t('contact.responseExpectation')}</p>
              <p>{t('contact.privacyNote')}</p>
            </div>
          </div>

          <ContactFormContainer />
        </div>
      </div>
    </section>
  );
}
