# Role Skill: UX Designer

> Role perspective for design system consistency, responsive UX, and accessibility.

## Role Context

The UX Designer ensures every screen follows the Reflect Studio design system, maintains visual consistency, and meets WCAG 2.1 AA accessibility standards.

## Design Philosophy

- **Brand gold (`--color-brand: #ffc347`)** reserved for CTAs, highlights, and badges only.
- Dark mode surfaces: `surface-900` → `surface-800` → `surface-700` → `surface-600`.
- Every color must have `dark:` Tailwind variant.

## Responsive Breakpoints

| Breakpoint | Width | Target |
|-----------|-------|--------|
| Default | < 640px | Mobile |
| `sm:` | ≥ 640px | Large phones |
| `md:` | ≥ 768px | Tablets |
| `lg:` | ≥ 1024px | Desktop |
| `xl:` | ≥ 1280px | Large desktop |

**Mobile-first**: write base styles for mobile, add `sm:`, `md:`, `lg:` overrides.

## UX Principles

### 1. Progressive Disclosure
- Essential information first; details on demand.
- Booking wizard: one step per screen, clear progress indicator.

### 2. Immediate Feedback
- Buttons: loading spinner + disabled during async.
- Skeleton loaders for data — never blank white space.
- Sonner toasts for confirmations (bottom-right, theme-aware).

### 3. Error Prevention
- Submit-only validation in drawers (avoid content shifts).
- Blur validation on standalone pages.
- Confirmation modal for destructive actions.

### 4. All Four LoadStatus States

| LoadStatus | UX Treatment |
|-----------|-------------|
| `None` | Initial — show CTA or nothing |
| `Loading` | Skeleton loaders, disabled interactions |
| `Success` | Render data (or empty state if empty) |
| `Failed` | Error message with retry action |

### 5. Mobile-First Interaction
- Modals become bottom-sheet drawers on mobile (Base UI Drawer).
- Touch targets: minimum 44×44px.
- Inputs: `text-base` (16px) to prevent iOS auto-zoom.

## Consult These Skills

| Task | Skill |
|------|-------|
| Color/typography/spacing | `ui-design-tokens.md` |
| Cards/buttons/inputs/loading | `ui-component-patterns.md` |
| Focus/ARIA/keyboard/motion | `ui-accessibility-wcag.md` |
| Translations | `i18n-conventions.md` |

## Checklist

- [ ] Mobile-first responsive design
- [ ] Design tokens — no hardcoded hex colors
- [ ] Brand color only for CTAs/highlights/badges
- [ ] All four LoadStatus states handled
- [ ] Skeleton loaders + empty states
- [ ] Loading spinner + disabled on action buttons
- [ ] Touch targets ≥ 44×44px on mobile
- [ ] Inputs at `text-base` (16px)
- [ ] Reduced motion respected
- [ ] Dark mode styles on all components
