import * as React from 'react';
import type { TFunction } from 'i18next';
import { Button } from '@/shared/ui/Button';
import type { LoadStatus } from '@/shared/model/types';

interface ContactFormProps {
  t: TFunction;
  name: string;
  email: string;
  message: string;
  website: string;
  onNameChange: (value: string) => void;
  onEmailChange: (value: string) => void;
  onMessageChange: (value: string) => void;
  onWebsiteChange: (value: string) => void;
  onSubmit: (e: React.FormEvent<HTMLFormElement>) => void;
  status: LoadStatus;
  error: string | null;
}

/**
 * Pure presentational contact form. Receives all state and handlers as props
 * so it can be rendered in tests / Storybook without any Redux wiring.
 */
export function ContactForm({
  t, name, email, message, website,
  onNameChange, onEmailChange, onMessageChange, onWebsiteChange,
  onSubmit, status, error,
}: ContactFormProps): React.JSX.Element {
  const isSubmitting = status === 'loading';
  const isSuccess = status === 'succeeded';
  const isError = status === 'failed';

  return (
    <form onSubmit={onSubmit} className="space-y-4 md:col-span-2" noValidate>
      <div className="hidden" aria-hidden="true">
        <label htmlFor="contact-website">{t('contact.formWebsite')}</label>
        <input
          id="contact-website"
          name="website"
          type="text"
          tabIndex={-1}
          autoComplete="off"
          value={website}
          onChange={(e) => onWebsiteChange(e.target.value)}
        />
      </div>
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
          maxLength={128}
          value={name}
          onChange={(e) => onNameChange(e.target.value)}
          disabled={isSubmitting}
          className="mt-2 w-full border-b border-paper/30 bg-transparent py-3 text-paper placeholder-paper/40 focus:border-brand focus:outline-none disabled:opacity-60"
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
          maxLength={256}
          value={email}
          onChange={(e) => onEmailChange(e.target.value)}
          disabled={isSubmitting}
          className="mt-2 w-full border-b border-paper/30 bg-transparent py-3 text-paper placeholder-paper/40 focus:border-brand focus:outline-none disabled:opacity-60"
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
          maxLength={4000}
          rows={5}
          value={message}
          onChange={(e) => onMessageChange(e.target.value)}
          disabled={isSubmitting}
          className="mt-2 w-full resize-y border-b border-paper/30 bg-transparent py-3 text-paper placeholder-paper/40 focus:border-brand focus:outline-none disabled:opacity-60"
        />
      </div>

      {isSuccess && (
        <p role="status" className="text-sm text-brand">
          {t('contact.formSuccess')}
        </p>
      )}
      {isError && (
        <p role="alert" className="text-sm text-red-300">
          {error ?? t('common.error')}
        </p>
      )}

      <Button
        type="submit"
        variant="outline"
        loading={isSubmitting}
        className="border-paper text-paper hover:bg-paper hover:text-ink"
      >
        {t('contact.formSubmit')}
      </Button>
    </form>
  );
}
