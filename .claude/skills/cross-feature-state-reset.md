# Skill: Cross-Feature State Reset via Listener Middleware

## When to Apply

Apply this pattern whenever:

- A Redux slice needs to **react to an action from a different feature** (e.g., reset on logout, refresh on login, clear cache when studio changes).
- You see a slice importing a thunk or action from `../../otherFeature/` — this is the code smell that triggers this refactoring.

## The Rule

> **Feature slices must NEVER subscribe to another feature's thunks or actions in `extraReducers`.**
>
> Cross-feature reactions belong in the **listener middleware layer** (`app/listeners/`).

## How to Implement

### Step 1: Ensure the reacting slice has its own reset/update action

```typescript
// features/booking/model/bookingSlice.ts
const bookingSlice = createSlice({
  name: 'booking',
  initialState,
  reducers: {
    resetBooking() {
      return initialState;
    },
  },
});
export const { resetBooking } = bookingSlice.actions;
```

### Step 2: Create or extend a listener in `app/listeners/`

```typescript
// app/listeners/logoutListener.ts
import { logoutThunk } from '../../features/auth';
import { resetBooking } from '../../features/booking';

export function registerLogoutListeners(startListening: AppStartListening) {
  startListening({
    actionCreator: logoutThunk.fulfilled,
    effect: (_action, listenerApi) => {
      listenerApi.dispatch(resetBooking());
    },
  });
}
```

### Step 3: Register the listener in `listenerMiddleware.ts`

```typescript
// app/listeners/listenerMiddleware.ts
import { registerLogoutListeners } from './logoutListener';
registerLogoutListeners(startListening);
```

### Step 4: Remove the cross-feature import from the slice

Remove the `logoutThunk` (or other feature's action) import and the `.addCase()` from `extraReducers`.

### Step 5: Update tests

- Slice tests: test the `resetXxx()` action directly (no cross-feature thunk imports).
- Listener tests: test that the listener dispatches the correct actions when the triggering action fires.

## Why This Matters

1. **FSD import boundaries** — `features/` cannot import from other `features/`.
2. **Single responsibility** — Each slice only knows about its own domain.
3. **Discoverability** — All cross-feature orchestration is in `app/listeners/`, not scattered across N slices.
4. **Testing** — Slice tests don't need cross-feature mocking.

## Reference

- ADR: `docs/architecture/ADR-CROSS-SLICE-LOGOUT-LISTENER.md`
- Implementation: `client/src/app/listeners/logoutListener.ts`
- Redux Toolkit docs: [createListenerMiddleware](https://redux-toolkit.js.org/api/createListenerMiddleware)
