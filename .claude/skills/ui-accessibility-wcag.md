# Skill: UI Accessibility (WCAG 2.1 AA)

> Focus management, ARIA attributes, keyboard navigation, and reduced motion.

## When to Apply

- Adding any interactive element (buttons, modals, forms, expandable content)
- Building navigation or multi-step flows

## Focus Management

```tsx
// Visible focus ring on all interactive elements
className="focus:ring-2 focus:ring-brand focus:outline-none"

// Wizard step heading — focus on mount for screen readers
const headingRef = useRef<HTMLHeadingElement>(null);
useEffect(() => { headingRef.current?.focus(); }, []);
<h2 ref={headingRef} tabIndex={-1}>Step Title</h2>
```

## Modals

- Focus trapping: `role="dialog"` or `role="alertdialog"`.
- ESC key dismissal.
- Use `shared/ui/Modal/Modal.tsx` (Base UI Drawer on mobile).

## Forms

- `<Label>` associated with every input via `htmlFor`.
- Field errors shown with `aria-describedby`.
- Password visibility toggles: `tabIndex={-1}` to not interrupt tab order.

## Semantic HTML

- Heading hierarchy: `h1 → h2 → h3` — never skip levels.
- Images: always `alt` text.
- Icon-only buttons: `aria-label`.
- Multi-step flows: `role="progressbar"` with `aria-valuenow`/`aria-valuemax`.

## Expandable Content

- `aria-expanded` attribute on trigger.
- Keyboard navigation (arrow keys for lists/menus).

## Motion

```css
/* Allowed: fade-up, scroll-reveal, hover lift, gallery zoom, button press */
/* Spinner: animate-spin (always allowed) */

@media (prefers-reduced-motion: reduce) {
  /* Disable all transforms/transitions */
  /* Keep only animate-spin */
  /* Scroll-reveal becomes instant opacity:1, no transform */
}
```

## Checklist

- [ ] Focus ring on all interactive elements
- [ ] `aria-label` on icon-only buttons
- [ ] `role="dialog"` + ESC dismissal on modals
- [ ] `<Label>` + `aria-describedby` for form errors
- [ ] Heading hierarchy maintained (`h1 → h2 → h3`)
- [ ] `alt` text on all images
- [ ] `prefers-reduced-motion` respected
- [ ] Password toggles: `tabIndex={-1}`
