import { createSlice } from '@reduxjs/toolkit';
import { sendContactMessage } from '../api/thunks';
import type { ContactState } from './types';

const initialState: ContactState = {
  status: 'idle',
  error: null,
};

export const contactSlice = createSlice({
  name: 'contact',
  initialState,
  reducers: {
    resetContact(state) {
      state.status = 'idle';
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(sendContactMessage.pending, (state) => {
        state.status = 'loading';
        state.error = null;
      })
      .addCase(sendContactMessage.fulfilled, (state) => {
        state.status = 'succeeded';
      })
      .addCase(sendContactMessage.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload ?? 'Failed to send message';
      });
  },
});

export const { resetContact } = contactSlice.actions;
export default contactSlice.reducer;
