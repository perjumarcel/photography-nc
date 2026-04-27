# Skill: Frontend Error Handling

> extractErrorMessage for thunks, Sonner toasts for display, err: unknown always.

## When to Apply

- Handling API errors in Redux thunks
- Displaying error messages to users
- Extracting structured error data from ProblemDetails responses

## Thunk Pattern

```typescript
import { extractErrorMessage } from '../../../shared/lib/thunkError';

export const createItem = createAsyncThunk(
  'items/create',
  async (dto: CreateItemDto, { rejectWithValue }) => {
    try {
      const { data } = await api.post<ItemDto>('/items', dto);
      return data;
    } catch (err: unknown) {                          // ALWAYS err: unknown
      if (axios.isAxiosError(err) && err.response?.status === 429) {
        return rejectWithValue('Too many requests. Please try again later.');
      }
      return rejectWithValue(
        extractErrorMessage(err, 'Failed to create item')  // ALWAYS with fallback
      );
    }
  }
);
```

## extractErrorMessage Priority Chain

`extractErrorMessage(err, fallback)` extracts in this order:
1. `err.response.data.detail` — ProblemDetails detail
2. `err.response.data.message` — Simple API error
3. `err.response.data.title` — ProblemDetails title
4. `err.message` — Axios/Error message
5. `String(err)` — Unknown type
6. `fallback` — Provided fallback string

## Slice Rejected Case

```typescript
.addCase(createItem.rejected, (state, action) => {
  state.status = LoadStatus.Failed;
  state.error = (action.payload as string)         // rejectWithValue message
    ?? action.error?.message                        // thrown error message
    ?? 'Operation failed';                          // ultimate fallback
});
```

## Toast Display (Sonner)

```typescript
import { toast } from 'sonner';

toast.success(t('common.saved'));
if (error) toast.error(error);
// Position: bottom-right, theme-aware (configured globally)
```

## Rules

1. **Always** `err: unknown` — never `err: any`.
2. **Always** `extractErrorMessage(err, 'fallback')` — never manual extraction.
3. Rate limit errors (429) handled specially before generic extraction.
4. Toast errors with Sonner — never `alert()` or `console.error()` for user-facing errors.
5. `extractErrorCode(err)` / `extractCorrelationId(err)` for programmatic handling.

## Checklist

- [ ] `err: unknown` in catch blocks — not `err: any`
- [ ] `extractErrorMessage(err, 'fallback')` in all thunks
- [ ] Rate limit (429) handled before generic error
- [ ] Slice rejected case handles `payload` and `error.message`
- [ ] Sonner toast for user-facing errors
