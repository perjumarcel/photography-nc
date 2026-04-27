# Role Skill: Frontend Engineer

> Role perspective for building and maintaining the React 19 + TypeScript SPA.

## Role Context

The Frontend Engineer owns the complete frontend lifecycle: Feature-Sliced Design architecture, state management, API integration, component patterns, and test coverage.

## Technology Stack

| Tool | Purpose |
|------|---------|
| React 19 | UI framework |
| TypeScript strict | Type safety |
| Redux Toolkit | State management |
| Vite | Build tool |
| Tailwind CSS 4 | Styling |
| Axios | HTTP client (`withCredentials`, Bearer, 401 refresh) |
| i18next | Internationalization (EN + RO) |
| Vitest / Cypress | Unit / E2E testing |

## FSD Import Rules (memorize)

```
app/     → Can import from: everything
pages/   → Can import from: features/, shared/
features/→ Can import from: shared/ ONLY
shared/  → Can import from: shared/ ONLY
```

**Never violate import boundaries.** Cross-feature reactions → `app/listeners/`.

## Container / Presenter Split

```typescript
// Container — ONLY place for Redux hooks
export default function FooContainer() {
  const dispatch = useAppDispatch();
  const { data, status, error } = useAppSelector(s => s.foo);
  return <FooView items={data} status={status} error={error} />;
}

// Presenter — ZERO Redux imports, pure props
export default function FooView({ items, status, error }: FooViewProps) { /* render-only */ }
```

## Size Limits & Naming

- **250 lines max** per component, **150 lines max** per hook.
- Components: `PascalCase.tsx`, Containers: `PascalCaseContainer.tsx`
- Hooks: `useCamelCase.ts`, Slices: `camelCaseSlice.ts`, APIs: `camelCaseApi.ts`

## TypeScript Strict Practices

```typescript
// No `any` — use `unknown` and narrow
function handleError(err: unknown): string {
  if (err instanceof Error) return err.message;
  return String(err);
}

// Exhaustive switch with `never`
default: { const _exhaustive: never = status; return _exhaustive; }
```

## Performance Practices

- Lazy load pages with `React.lazy()` + `Suspense`.
- Memoize selectors with `createSelector`.
- Split containers to minimize re-render scope.
- `loading="lazy"` on below-fold images.

## Consult These Skills

| Task | Skill |
|------|-------|
| New feature | `frontend-feature-slice.md` |
| Async state | `redux-async-state.md` |
| Error handling | `frontend-error-handling.md` |
| Cross-feature | `cross-feature-state-reset.md` |
| Design tokens | `ui-design-tokens.md` |
| Components | `ui-component-patterns.md` |
| Accessibility | `ui-accessibility-wcag.md` |
| Translations | `i18n-conventions.md` |
| Vitest tests | `testing-frontend-vitest.md` |
| E2E tests | `testing-e2e-cypress.md` |
| Security | `security-checklist.md` |

## Checklist (every PR)

- [ ] FSD import boundaries respected
- [ ] Container/Presenter split for Redux-connected components
- [ ] `LoadStatus` state shape on all async slices
- [ ] `err: unknown` + `extractErrorMessage()` in all thunks
- [ ] `reset{Feature}` action exported and in logout listener
- [ ] i18n keys added for both EN + RO
- [ ] Components under 250 lines, hooks under 150 lines
- [ ] Test IDs from centralized `testIds.ts`
- [ ] Vitest tests for new slices and utilities
- [ ] No `any` types in new code
- [ ] `cn()` for all conditional class merging
