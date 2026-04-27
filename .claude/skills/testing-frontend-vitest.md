# Skill: Frontend Testing (Vitest)

> Redux slice tests, utility testing, and component rendering with Vitest.

## When to Apply

- Testing Redux slices (all async state transitions)
- Testing utility functions and selectors
- Verifying component rendering

## Slice Test Template

```typescript
import { describe, it, expect } from 'vitest';
import reducer, { resetFeature } from './featureSlice';
import { fetchItems } from '../api/featureApi';
import { LoadStatus } from '../../../shared/model/loadStatus';

const initialState = {
  data: [],
  status: LoadStatus.None,
  error: null,
};

describe('feature reducer', () => {
  it('should return initial state', () => {
    expect(reducer(undefined, { type: '@@INIT' })).toEqual(initialState);
  });

  it('should handle resetFeature', () => {
    const loaded = { data: [{ id: '1' }], status: LoadStatus.Success, error: null };
    expect(reducer(loaded, resetFeature())).toEqual(initialState);
  });

  it('should handle fetchItems.pending', () => {
    const state = reducer(
      { ...initialState, error: 'old error' },
      { type: fetchItems.pending.type }
    );
    expect(state.status).toBe(LoadStatus.Loading);
    expect(state.error).toBeNull();
  });

  it('should handle fetchItems.fulfilled', () => {
    const state = reducer(initialState, {
      type: fetchItems.fulfilled.type,
      payload: [{ id: '1', name: 'Test' }],
    });
    expect(state.status).toBe(LoadStatus.Success);
  });

  it('should handle fetchItems.rejected', () => {
    const state = reducer(initialState, {
      type: fetchItems.rejected.type,
      payload: 'Network error',
    });
    expect(state.status).toBe(LoadStatus.Failed);
    expect(state.error).toBe('Network error');
  });

  it('should handle fetchItems.rejected with undefined payload', () => {
    const state = reducer(initialState, {
      type: fetchItems.rejected.type,
      payload: undefined,
      error: { message: 'Unknown error' },
    });
    expect(state.error).toBe('Unknown error');
  });
});
```

## Rules

- Use `describe` + `it` blocks (not `test`).
- Test **all** reducer cases: pending, fulfilled, rejected.
- Test with **undefined payload** for rejected (fallback path).
- Test **reset action** returns initial state.
- Test utility functions with edge cases.
- `vi.mock()` for module mocking when needed.
- Run: `cd client && npx vitest run` — never `vitest` alone (avoids watch mode).

## Checklist

- [ ] All async state transitions tested (pending/fulfilled/rejected)
- [ ] Rejected with undefined payload tested (fallback)
- [ ] Reset action returns initial state
- [ ] Utility functions tested with edge cases
- [ ] `describe` + `it` pattern (not `test`)
