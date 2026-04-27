import { describe, expect, it } from 'vitest';
import contactReducer, { resetContact } from './contactSlice';
import { sendContactMessage } from '../api/thunks';

describe('contactSlice', () => {
  it('starts idle', () => {
    const state = contactReducer(undefined, { type: '@@init' });
    expect(state.status).toBe('idle');
    expect(state.error).toBeNull();
  });

  it('marks loading on pending', () => {
    const state = contactReducer(undefined, { type: sendContactMessage.pending.type });
    expect(state.status).toBe('loading');
  });

  it('marks succeeded on fulfilled', () => {
    const state = contactReducer(undefined, { type: sendContactMessage.fulfilled.type });
    expect(state.status).toBe('succeeded');
    expect(state.error).toBeNull();
  });

  it('captures error on rejected', () => {
    const state = contactReducer(undefined, {
      type: sendContactMessage.rejected.type,
      payload: 'boom',
    });
    expect(state.status).toBe('failed');
    expect(state.error).toBe('boom');
  });

  it('resetContact clears state', () => {
    const seeded = contactReducer(undefined, { type: sendContactMessage.fulfilled.type });
    const cleared = contactReducer(seeded, resetContact());
    expect(cleared.status).toBe('idle');
  });
});
