import * as React from 'react';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button } from '@/shared/ui/Button';

/**
 * Contact page. Mirrors `Contact/Index.cshtml` — info column on the left,
 * contact form on the right. Stacks on mobile.
 *
 * The form does not currently POST anywhere — the submit handler simulates
 * success and is the integration point for a future `/api/public/contact`.
 */
export function ContactPage(): React.JSX.Element {
  const { t } = useTranslation();
  const [submitted, setSubmitted] = useState(false);
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');

  const onSubmit = (e: React.FormEvent<HTMLFormElement>): void => {
    e.preventDefault();
    setSubmitted(true);
    setName('');
    setEmail('');
    setMessage('');
  };

  return (
    <section className="bg-ink-soft text-paper">
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

          {/* Form */}
          <form onSubmit={onSubmit} className="space-y-4 md:col-span-2" noValidate>
            <div>
              <label htmlFor="contact-name" className="block text-[0.7rem] uppercase tracking-[0.25em] text-paper/70">
                {t('contact.formName')}
              </label>
              <input
                id="contact-name"
                name="name"
                type="text"
                required
                autoComplete="name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="mt-2 w-full border-b border-paper/30 bg-transparent py-3 text-paper placeholder-paper/40 focus:border-brand focus:outline-none"
              />
            </div>
            <div>
              <label htmlFor="contact-email" className="block text-[0.7rem] uppercase tracking-[0.25em] text-paper/70">
                {t('contact.formEmail')}
              </label>
              <input
                id="contact-email"
                name="email"
                type="email"
                required
                autoComplete="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="mt-2 w-full border-b border-paper/30 bg-transparent py-3 text-paper placeholder-paper/40 focus:border-brand focus:outline-none"
              />
            </div>
            <div>
              <label htmlFor="contact-message" className="block text-[0.7rem] uppercase tracking-[0.25em] text-paper/70">
                {t('contact.formMessage')}
              </label>
              <textarea
                id="contact-message"
                name="message"
                required
                rows={5}
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                className="mt-2 w-full resize-y border-b border-paper/30 bg-transparent py-3 text-paper placeholder-paper/40 focus:border-brand focus:outline-none"
              />
            </div>

            {submitted && (
              <p role="status" className="text-sm text-brand">
                {t('contact.formSuccess')}
              </p>
            )}

            <Button
              type="submit"
              variant="outline"
              className="border-paper text-paper hover:bg-paper hover:text-ink"
            >
              {t('contact.formSubmit')}
            </Button>
          </form>
        </div>
      </div>
    </section>
  );
}
