# Skill: Frontend Feature Slice (FSD)

> Use this pattern when scaffolding a new feature or adding a new domain area to the frontend.

## When to Apply

- Adding a new feature (e.g., notifications, promotions)
- Restructuring an existing feature that grew beyond its boundaries
- Any new domain-scoped UI + state + API layer

## Directory Structure

```
client/src/features/{featureName}/
  api/
    {featureName}Api.ts      ← createAsyncThunk calls + API types
  model/
    {featureName}Slice.ts    ← createSlice + extraReducers
    types.ts                 ← TypeScript interfaces for this feature
    selectors.ts             ← Memoized selectors (optional)
  ui/
    {Name}Container.tsx      ← Redux-connected container
    {Name}.tsx               ← Pure presenter component
    {Name}.test.tsx           ← Vitest component test (optional)
  index.ts                   ← Public API barrel export
```

## API Thunk Template

```typescript
// features/{name}/api/{name}Api.ts
import { createAsyncThunk } from '@reduxjs/toolkit';
import api from '../../../shared/api/client';
import { extractErrorMessage } from '../../../shared/lib/thunkError';

export interface FooDto {
  id: string;
  name: string;
}

export const fetchFoos = createAsyncThunk(
  '{name}/fetchFoos',
  async (studioId: string, { rejectWithValue }) => {
    try {
      const { data } = await api.get<FooDto[]>(`/admin/${studioId}/foos`);
      return data;
    } catch (err: unknown) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load foos'));
    }
  }
);
```

Rules:
- Always `err: unknown` — never `err: any`.
- Always use `extractErrorMessage()` from `shared/lib/thunkError`.
- Thunk name: `'{sliceName}/{actionName}'`.

## Slice Template

```typescript
// features/{name}/model/{name}Slice.ts
import { createSlice } from '@reduxjs/toolkit';
import { LoadStatus } from '../../../shared/model/loadStatus';
import { fetchFoos } from '../api/{name}Api';
import type { FooDto } from '../api/{name}Api';

interface FooState {
  data: FooDto[];
  status: LoadStatus;
  error: string | null;
}

const initialState: FooState = {
  data: [],
  status: LoadStatus.None,
  error: null,
};

const fooSlice = createSlice({
  name: '{name}',
  initialState,
  reducers: {
    resetFoo: () => initialState,
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchFoos.pending, (state) => {
        state.status = LoadStatus.Loading;
        state.error = null;
      })
      .addCase(fetchFoos.fulfilled, (state, action) => {
        state.status = LoadStatus.Success;
        state.data = action.payload;
      })
      .addCase(fetchFoos.rejected, (state, action) => {
        state.status = LoadStatus.Failed;
        state.error = (action.payload as string) ?? 'Failed to load';
      });
  },
});

export const { resetFoo } = fooSlice.actions;
export default fooSlice.reducer;
```

Rules:
- Always expose a `reset{Name}` action for logout cleanup.
- Use `LoadStatus` enum from `shared/model/loadStatus` — not custom strings.
- Never subscribe to another feature's thunks in `extraReducers`.

## Container / Presenter Split

```typescript
// Container — reads Redux, dispatches, passes props
import { useAppDispatch, useAppSelector } from '../../../app/hooks';
import { fetchFoos } from '../api/{name}Api';
import FooList from './FooList';

export default function FooListContainer() {
  const dispatch = useAppDispatch();
  const { data, status, error } = useAppSelector((s) => s.foo);
  // dispatch, transform, pass down
  return <FooList items={data} status={status} error={error} />;
}

// Presenter — pure props, no Redux hooks
interface FooListProps {
  items: FooDto[];
  status: LoadStatus;
  error: string | null;
}

export default function FooList({ items, status, error }: FooListProps) {
  // render only — no useAppDispatch, no useAppSelector
}
```

## Barrel Export

```typescript
// features/{name}/index.ts
export { default as fooReducer } from './model/{name}Slice';
export { resetFoo } from './model/{name}Slice';
export { fetchFoos } from './api/{name}Api';
```

## Checklist

- [ ] `api/`, `model/`, `ui/` directories created
- [ ] API thunks use `extractErrorMessage(err, 'fallback')`
- [ ] Slice has `LoadStatus` state shape
- [ ] `reset{Name}` action exported for logout cleanup
- [ ] Container/Presenter split for Redux-connected components
- [ ] No imports from other `features/` — only from `shared/`
- [ ] No imports from `app/hooks` in Presenter components
- [ ] Barrel `index.ts` exports reducer + public actions/thunks
- [ ] Register reducer in `app/store.ts`
- [ ] Register reset action in `app/listeners/logoutListener.ts`
- [ ] i18n keys added for EN + RO
- [ ] Max 250 lines per component, 150 per hook
