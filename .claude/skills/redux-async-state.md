# Skill: Redux Async State Management

> Use this pattern for all Redux async operations with createAsyncThunk.

## When to Apply

- Adding any API call that updates Redux state
- Creating a new slice with async data fetching
- Handling loading, success, and error states

## State Shape (mandatory)

```typescript
import { LoadStatus } from '../../../shared/model/loadStatus';

// LoadStatus enum values: None | Loading | Success | Failed

interface FeatureState {
  data: ItemDto[];              // or single item, or null
  status: LoadStatus;           // async operation status
  error: string | null;         // error message for UI display
}

const initialState: FeatureState = {
  data: [],
  status: LoadStatus.None,
  error: null,
};
```

Every async-dependent state must follow this shape. Never use custom string status values.

## Thunk Pattern

```typescript
import { createAsyncThunk } from '@reduxjs/toolkit';
import api from '../../../shared/api/client';
import { extractErrorMessage } from '../../../shared/lib/thunkError';

export const fetchItems = createAsyncThunk(
  'featureName/fetchItems',                      // namespace/action
  async (studioId: string, { rejectWithValue }) => {
    try {
      const { data } = await api.get<ItemDto[]>(`/admin/${studioId}/items`);
      return data;
    } catch (err: unknown) {                     // ALWAYS err: unknown
      return rejectWithValue(
        extractErrorMessage(err, 'Failed to load items')  // ALWAYS extractErrorMessage
      );
    }
  }
);
```

### Error Handling Rules

- **Always** `err: unknown` — never `err: any`.
- **Always** use `extractErrorMessage(err, 'fallback message')` from `shared/lib/thunkError`.
- `extractErrorMessage` handles: ProblemDetails, AxiosError, Error, string, and unknown types.
- For rate limiting: check `axios.isAxiosError(err) && err.response?.status === 429` before generic handling.

## Slice extraReducers Pattern

```typescript
const featureSlice = createSlice({
  name: 'featureName',
  initialState,
  reducers: {
    resetFeature: () => initialState,  // Always expose reset for logout cleanup
    clearError(state) {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchItems.pending, (state) => {
        state.status = LoadStatus.Loading;
        state.error = null;
      })
      .addCase(fetchItems.fulfilled, (state, action) => {
        state.status = LoadStatus.Success;
        state.data = action.payload;
      })
      .addCase(fetchItems.rejected, (state, action) => {
        state.status = LoadStatus.Failed;
        state.error = (action.payload as string) ?? action.error?.message ?? 'Operation failed';
      });
  },
});
```

## Cross-Feature Reactions

**Never** subscribe to another feature's thunks in `extraReducers`.

```typescript
// BAD — violates FSD import boundaries
import { logoutThunk } from '../../auth/model/thunks/logout';
extraReducers: (builder) => {
  builder.addCase(logoutThunk.fulfilled, () => initialState); // WRONG
};

// GOOD — use listener middleware
// In app/listeners/logoutListener.ts:
startListening({
  actionCreator: logoutThunk.fulfilled,
  effect: (_action, listenerApi) => {
    listenerApi.dispatch(resetFeature());
  },
});
```

## Selector Pattern

```typescript
// In model/selectors.ts
import type { RootState } from '../../../app/store';

export const selectItems = (state: RootState) => state.featureName.data;
export const selectItemsStatus = (state: RootState) => state.featureName.status;
export const selectItemsError = (state: RootState) => state.featureName.error;
export const selectIsLoading = (state: RootState) =>
  state.featureName.status === LoadStatus.Loading;
```

## Checklist

- [ ] State uses `LoadStatus` enum from `shared/model/loadStatus`
- [ ] Thunk uses `err: unknown` + `extractErrorMessage()`
- [ ] Thunk name follows `'sliceName/actionName'` convention
- [ ] `pending` case: set Loading + clear error
- [ ] `fulfilled` case: set Success + store data
- [ ] `rejected` case: set Failed + extract error message
- [ ] Slice exposes `reset{Name}` action
- [ ] No cross-feature thunk imports in `extraReducers`
- [ ] Cross-feature cleanup in `app/listeners/`
- [ ] Selectors typed with `RootState`
