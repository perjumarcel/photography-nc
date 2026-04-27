# Skill: UI Design Tokens

> Color tokens, typography scale, and spacing system — never hardcode values.

## When to Apply

- Setting colors, fonts, or spacing on any component
- Creating new UI elements that need design consistency

## Color Tokens

```css
/* Brand — CTAs, highlights, badges ONLY (never large fills or body text) */
--color-brand: #ffc347;
--color-brand-dark: #e6b800;        /* hover/active */
--color-brand-light: #fff8e1;       /* focus rings, subtle bg */
--color-brand-text: #1a1a1a;        /* text on brand bg */

/* Surfaces (dark mode depth hierarchy) */
--color-surface-900: #0a0a0a;       /* darkest bg */
--color-surface-800: #141414;       /* card bg */
--color-surface-700: #1e1e1e;       /* elevated */
--color-surface-600: #2a2a2a;       /* borders */
```

Use Tailwind classes that reference these tokens. **Never write raw hex values.**

## Typography Scale

| Element | Classes |
|---------|---------|
| Hero H1 | `text-4xl sm:text-5xl lg:text-6xl font-bold leading-[1.1] tracking-tight` |
| Section H2 | `text-3xl sm:text-4xl font-bold` |
| Card H3 | `text-xl font-bold` |
| Body | `text-base leading-relaxed` |
| Caption | `text-sm` |

## Spacing System

- Tailwind 4px grid: `p-1` (4px), `p-2` (8px), `p-4` (16px), `p-5` (20px), `p-6` (24px).
- Cards: `p-5` or `p-6` with `gap-4` or `gap-6`.
- Sections: `py-12 sm:py-16 lg:py-20`.
- Never use arbitrary values (`p-[13px]`).

## Class Merging

Always use `cn()` from tailwind-merge + clsx:

```tsx
import { cn } from '../../shared/lib/utils';

<div className={cn('rounded-xl p-5', isActive && 'border-brand', className)} />
```

Use `class-variance-authority` (CVA) for component variants.

## Checklist

- [ ] Design tokens used — no hardcoded hex colors
- [ ] Brand color only for CTAs/highlights/badges
- [ ] Typography follows the scale
- [ ] `cn()` for all conditional class merging
- [ ] Dark mode variants on all color classes
