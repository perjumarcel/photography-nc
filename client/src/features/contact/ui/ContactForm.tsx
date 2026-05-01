import * as React from 'react';
import type { TFunction } from 'i18next';
import { Button } from '@/shared/ui/Button';
import type { LoadStatus } from '@/shared/model/types';

interface ContactFormProps {
  t: TFunction;
  name: string;
  email: string;
  phone: string;
  eventType: string;
  preferredDate: string;
  venue: string;
  estimatedBudgetRange: string;
  message: string;
  website: string;
  onNameChange: (value: string) => void;
  onEmailChange: (value: string) => void;
  onPhoneChange: (value: string) => void;
  onEventTypeChange: (value: string) => void;
  onPreferredDateChange: (value: string) => void;
  onVenueChange: (value: string) => void;
  onEstimatedBudgetRangeChange: (value: string) => void;
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
  t, name, email, phone, eventType, preferredDate, venue, estimatedBudgetRange, message, website,
  onNameChange, onEmailChange, onPhoneChange, onEventTypeChange, onPreferredDateChange,
  onVenueChange, onEstimatedBudgetRangeChange, onMessageChange, onWebsiteChange,
  onSubmit, status, error,
}: ContactFormProps): React.JSX.Element {
  const isSubmitting = status === 'loading';
  const isSuccess = status === 'succeeded';
  const isError = status === 'failed';

  return (
    <form onSubmit={onSubmit} className="space-y-4 md:col-span-2" noValidate>
      <div className="absolute -left-[9999px] h-px w-px overflow-hidden">
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
      <div className="grid gap-4 sm:grid-cols-2">
        <ContactInput id="contact-name" name="name" label={t('contact.formName')} value={name}
          onChange={onNameChange} disabled={isSubmitting} required autoComplete="name" maxLength={128} />
        <ContactInput id="contact-email" name="email" label={t('contact.formEmail')} value={email}
          onChange={onEmailChange} disabled={isSubmitting} required type="email" autoComplete="email" maxLength={256} />
        <ContactInput id="contact-phone" name="phone" label={t('contact.formPhone')} value={phone}
          onChange={onPhoneChange} disabled={isSubmitting} type="tel" autoComplete="tel" maxLength={64} />
        <ContactInput id="contact-event-type" name="eventType" label={t('contact.formEventType')} value={eventType}
          onChange={onEventTypeChange} disabled={isSubmitting} autoComplete="off" maxLength={256} />
        <ContactInput id="contact-date" name="preferredDate" label={t('contact.formPreferredDate')} value={preferredDate}
          onChange={onPreferredDateChange} disabled={isSubmitting} type="date" maxLength={256} />
        <ContactInput id="contact-venue" name="venue" label={t('contact.formVenue')} value={venue}
          onChange={onVenueChange} disabled={isSubmitting} autoComplete="street-address" maxLength={256} />
      </div>
      <ContactInput
        id="contact-budget"
        name="estimatedBudgetRange"
        label={t('contact.formBudget')}
        value={estimatedBudgetRange}
        onChange={onEstimatedBudgetRangeChange}
        disabled={isSubmitting}
        maxLength={256}
      />
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

interface ContactInputProps {
  id: string;
  name: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  disabled: boolean;
  required?: boolean;
  type?: React.HTMLInputTypeAttribute;
  autoComplete?: string;
  maxLength?: number;
}

function ContactInput({
  id, name, label, value, onChange, disabled, required = false, type = 'text', autoComplete, maxLength,
}: ContactInputProps): React.JSX.Element {
  return (
    <div>
      <label htmlFor={id} className="block text-[0.7rem] uppercase tracking-[0.25em] text-paper/70">
        {label}
      </label>
      <input
        id={id}
        name={name}
        type={type}
        required={required}
        autoComplete={autoComplete}
        maxLength={maxLength}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        disabled={disabled}
        className="mt-2 w-full border-b border-paper/30 bg-transparent py-3 text-paper placeholder-paper/40 focus:border-brand focus:outline-none disabled:opacity-60"
      />
    </div>
  );
}
