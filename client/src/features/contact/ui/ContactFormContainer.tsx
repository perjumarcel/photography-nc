import * as React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { sendContactMessage } from '../api/thunks';
import { resetContact } from '../model/contactSlice';
import { ContactForm } from './ContactForm';

/**
 * Container that owns the controlled inputs, dispatches the thunk and pipes
 * status / errors from the Redux slice to the presentational <ContactForm />.
 *
 * This is the integration point for the backend `/api/public/contact` endpoint.
 */
export function ContactFormContainer(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const { status, error } = useAppSelector((s) => s.contact);

  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [eventType, setEventType] = useState('');
  const [preferredDate, setPreferredDate] = useState('');
  const [venue, setVenue] = useState('');
  const [estimatedBudgetRange, setEstimatedBudgetRange] = useState('');
  const [message, setMessage] = useState('');
  const [website, setWebsite] = useState('');

  // Clear any prior status when the form is re-mounted (e.g. after navigating away).
  useEffect(() => () => { dispatch(resetContact()); }, [dispatch]);

  const onSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();
    const action = await dispatch(sendContactMessage({
      name,
      email,
      phone,
      eventType,
      preferredDate,
      venue,
      estimatedBudgetRange,
      message,
      sourcePage: window.location.pathname,
      website,
    }));
    if (sendContactMessage.fulfilled.match(action)) {
      setName('');
      setEmail('');
      setPhone('');
      setEventType('');
      setPreferredDate('');
      setVenue('');
      setEstimatedBudgetRange('');
      setMessage('');
      setWebsite('');
    }
  };

  return (
    <ContactForm
      t={t}
      name={name}
      email={email}
      phone={phone}
      eventType={eventType}
      preferredDate={preferredDate}
      venue={venue}
      estimatedBudgetRange={estimatedBudgetRange}
      message={message}
      website={website}
      onNameChange={setName}
      onEmailChange={setEmail}
      onPhoneChange={setPhone}
      onEventTypeChange={setEventType}
      onPreferredDateChange={setPreferredDate}
      onVenueChange={setVenue}
      onEstimatedBudgetRangeChange={setEstimatedBudgetRange}
      onMessageChange={setMessage}
      onWebsiteChange={setWebsite}
      onSubmit={onSubmit}
      status={status}
      error={error}
    />
  );
}
