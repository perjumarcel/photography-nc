# Skill: i18n Conventions

> Use this pattern for all user-visible text in the frontend.

## When to Apply

- Adding any user-visible text (labels, messages, titles, tooltips)
- Creating new components with text content
- Modifying existing text content

## Setup

The app uses `i18next` + `react-i18next` with two languages:
- **RO** (Romanian) — default language
- **EN** (English) — fallback language

Language preference stored in `localStorage` under key `reflect-studio-lang`.

## Translation Files

```
client/src/i18n/locales/
  en.json    ← English translations
  ro.json    ← Romanian translations (default)
```

## Using Translations

### In Components

```tsx
import { useTranslation } from 'react-i18next';

function MyComponent() {
  const { t } = useTranslation();

  return (
    <div>
      <h2>{t('section.title')}</h2>
      <p>{t('section.description')}</p>
      <button>{t('common.save')}</button>
    </div>
  );
}
```

### With Interpolation

```tsx
// In JSON: "greeting": "Hello, {{name}}!"
<p>{t('greeting', { name: user.firstName })}</p>

// In JSON: "itemCount": "{{count}} items"
<p>{t('itemCount', { count: items.length })}</p>
```

### Outside Components

```typescript
import i18n from '../../i18n';

const message = i18n.t('notifications.bookingConfirmed');
```

## Key Structure

Organize keys by feature/section with dot notation:

```json
{
  "hero": {
    "subline": "...",
    "primaryCta": "Book a session"
  },
  "booking": {
    "wizard": {
      "stepService": "Choose a service",
      "stepDateTime": "Pick date & time",
      "stepAddOns": "Add extras",
      "stepReview": "Review & confirm"
    },
    "status": {
      "pending": "Pending",
      "confirmed": "Confirmed",
      "cancelled": "Cancelled"
    }
  },
  "admin": {
    "calendar": {
      "title": "Calendar",
      "noBookings": "No bookings for this day"
    }
  },
  "common": {
    "save": "Save",
    "cancel": "Cancel",
    "delete": "Delete",
    "loading": "Loading...",
    "error": "Something went wrong"
  },
  "validation": {
    "required": "This field is required",
    "invalidEmail": "Please enter a valid email",
    "minLength": "Must be at least {{min}} characters"
  }
}
```

### Key Naming Rules

1. **Feature scope first**: `booking.wizard.stepService`, `admin.calendar.title`
2. **Common keys shared**: `common.save`, `common.cancel`, `common.loading`
3. **Validation keys centralized**: `validation.required`, `validation.invalidEmail`
4. **Status/enum keys grouped**: `booking.status.pending`, `booking.status.confirmed`
5. **CTA keys descriptive**: `hero.primaryCta`, `certificates.buyCertificate`

## Adding New Translations

1. Add the key to **both** `en.json` and `ro.json`.
2. Use the same key structure in both files.
3. Never add to one language without the other.
4. Fallback is English — but both must exist for quality.

## Rules

- **Never hardcode user-visible strings** — always use `t('key')`.
- **Both EN + RO required** for every key.
- **RO is the default** — it's what most users see first.
- Error messages from the backend arrive in English; frontend displays them as-is or maps to i18n keys.
- Placeholder text, `aria-label`, and `title` attributes also use `t()`.
- Don't translate `data-testid` values — those are code identifiers.

## Checklist

- [ ] All user-visible text uses `t('key')`
- [ ] Key added to both `en.json` and `ro.json`
- [ ] Key follows `feature.section.element` naming
- [ ] Interpolation used for dynamic values: `{{variable}}`
- [ ] Common text reuses `common.*` keys
- [ ] Validation messages use `validation.*` keys
- [ ] `aria-label` and `placeholder` also translated
