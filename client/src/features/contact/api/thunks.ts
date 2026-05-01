import { createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '@/shared/api/client';
import { extractErrorMessage } from '@/shared/lib/extractErrorMessage';
import type { ContactMessage } from '../model/types';

/**
 * POSTs a public contact form submission to the backend. The endpoint accepts
 * `{ name, email, phone, eventType, preferredDate, venue, estimatedBudgetRange, message }`
 * and returns `204 No Content` on success.
 */
export const sendContactMessage = createAsyncThunk<void, ContactMessage, { rejectValue: string }>(
  'contact/send',
  async (payload, { rejectWithValue }) => {
    try {
      await api.post('/public/contact', payload);
    } catch (err: unknown) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to send message'));
    }
  },
);
