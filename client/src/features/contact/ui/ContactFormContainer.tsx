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
  const [message, setMessage] = useState('');

  // Clear any prior status when the form is re-mounted (e.g. after navigating away).
  useEffect(() => () => { dispatch(resetContact()); }, [dispatch]);

  const onSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();
    const action = await dispatch(sendContactMessage({ name, email, message }));
    if (sendContactMessage.fulfilled.match(action)) {
      setName('');
      setEmail('');
      setMessage('');
    }
  };

  return (
    <ContactForm
      t={t}
      name={name}
      email={email}
      message={message}
      onNameChange={setName}
      onEmailChange={setEmail}
      onMessageChange={setMessage}
      onSubmit={onSubmit}
      status={status}
      error={error}
    />
  );
}
