# Skill: UI Component Patterns

> Cards, buttons, inputs, loading states, and empty states.

## When to Apply

- Building any new UI component
- Ensuring loading and empty states are handled

## Cards

```tsx
<div className="rounded-xl p-5 border border-zinc-100 dark:border-zinc-700
  hover:border-brand/20 hover:shadow-md transition-shadow">
  {/* content */}
</div>
```

## Buttons

```tsx
<Button className="rounded-lg py-2.5 font-semibold" disabled={isLoading}>
  {isLoading ? <Spinner className="w-4 h-4" /> : null}
  {label}
</Button>
```

- `rounded-lg` — never `rounded-full` for buttons.
- Every submit/action button shows **spinner + disabled** during loading.
- Use existing `Button` from `shared/ui/Button`.

## Icons

- SVG line icons, `strokeWidth="1.5"`, `w-6 h-6` or `w-7 h-7`.
- No emoji in UI.
- Icon-only buttons must have `aria-label`.

## Inputs

- **Always `text-base` (16px)** — prevents iOS Safari auto-zoom on focus.
- Never use `text-sm` on input elements.
- Use `shared/ui/Input`, `shared/ui/PasswordInput`, `shared/ui/PhoneInput`.

## Loading States

- **Skeleton loaders** for data-dependent content.
- Never blank white space during fetch.
- Check shadcn/ui for existing skeleton components.

## Empty States

Always show a clear empty state with action prompt:

```tsx
{items.length === 0 ? (
  <div className="flex flex-col items-center justify-center py-12 text-center">
    <IconPlaceholder className="w-12 h-12 text-zinc-400 mb-4" />
    <h3 className="text-lg font-semibold mb-1">{t('items.empty.title')}</h3>
    <p className="text-sm text-zinc-500 mb-4">{t('items.empty.description')}</p>
    <Button onClick={onAdd}>{t('items.empty.cta')}</Button>
  </div>
) : (
  <ItemList items={items} />
)}
```

## Component Size Limits

- Max **250 lines per component** — extract sub-components.
- Max **150 lines per hook** — extract utilities or split.

## Checklist

- [ ] Check shadcn/ui before building from scratch
- [ ] Loading spinner + disabled on submit buttons
- [ ] Skeleton loaders for async content
- [ ] Empty state for empty lists
- [ ] `text-base` (16px) on all inputs
- [ ] Component under 250 lines
