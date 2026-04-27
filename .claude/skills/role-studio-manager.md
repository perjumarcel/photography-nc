# Role Skill: Studio Manager (Domain Expert)

> Encode business domain knowledge, studio configuration, and booking workflow conventions for Reflect Studio.

## Role Context

The Studio Manager understands the photography studio business domain. This role ensures the software correctly models real-world studio operations: booking flows, session management, pricing, promotions, gallery delivery, and client relationships.

---

## Business Domain Overview

Reflect Studio is a **self-portrait photography studio** in Chișinău, Moldova. Key business characteristics:

- **No photographer present** — clients use the studio independently
- **Self-guided sessions** with professional equipment, lighting, and live preview
- **Time-slotted bookings** with session durations and buffer time between sessions
- **Multi-room capability** — each room is a bookable resource
- **Gallery-based photo delivery** — password-protected, expiring galleries

---

## Booking Lifecycle

### Status Machine

```
Pending → Confirmed → Completed
Pending → Cancelled
Pending → Expired (auto, past confirmation window)
Confirmed → Completed
Confirmed → Cancelled
Confirmed → NoShow
Completed → Confirmed (admin correction)
NoShow → Confirmed (admin correction)
```

### Business Rules

| Rule | Implementation |
|------|---------------|
| Booking creates a time hold | `StartUtc` + `DurationMinutes` = `SessionEndUtc` |
| Buffer between sessions | `SessionEndUtc` + `BufferMinutes` = `BlockedEndUtc` |
| Overlap prevention | Check `BlockedEndUtc > newStart AND StartUtc < newEnd` |
| Pending expiration | Configurable window, auto-expired by `ExpiredDataCleanupService` |
| Confirmation window | Studio setting — not hardcoded |
| Price snapshot | Price locked at booking creation — never changes |

### Booking Confirmation Flow

1. Client books online → status: `Pending`
2. Confirmation email sent with action token
3. Client clicks confirm link → status: `Confirmed`
4. Reminder email sent before session
5. After session → admin marks `Completed` or `NoShow`

---

## Pricing Model

### Session Pricing

- Each **service** (session type) has a base price
- **Add-ons** are optional extras (props, outfits, etc.)
- **Promotions** (promo codes, gift certificates) apply discounts
- Final price = `BasePrice + AddOns - Discounts` (minimum 0)

### Critical Pricing Rules

1. **Snapshot at booking creation** — store `BasePriceSnapshot`, `AddOnPriceSnapshot`
2. **Never recompute** from current catalog after booking is created
3. **Client-side calculation is preview only** — backend always re-validates
4. **Currency**: `MDL` (Moldovan Leu) — 3-character ISO code
5. **Monetary values**: always `>= 0`, use `Math.Max(0, total)` after discount

---

## Promotions

### Promo Codes

- Studio-created discount codes
- Can be percentage or fixed amount
- May have usage limits, expiration dates
- Normalized via `PromotionCode` value object (uppercase, trimmed)

### Gift Certificates

- Purchased by clients, delivered via email
- Fixed monetary value
- Single-use — concurrency token prevents double redemption
- Must be restored if booking is cancelled

### Redemption Flow

```
1. Client enters code during booking
2. Backend validates: exists? not expired? not redeemed?
3. Code redeemed within same transaction as booking creation
4. If booking cancelled → code restored via RestoreAfterCancellation()
```

---

## Gallery Delivery

### Gallery Lifecycle

```
Created → Active (photos uploaded) → Shared → Expired → Deleted
```

### Business Rules

| Rule | Implementation |
|------|---------------|
| Access via unique token | `gallery/{accessToken}` URL |
| Password protection | Optional, stored as hash |
| Expiration | Configurable per gallery, auto-expired by background service |
| ZIP download | Generated on-demand for <500 items, cached for larger |
| Sharing | Admin explicitly triggers — never auto-send on creation |
| Last shared tracking | `LastSharedAtUtc` warns if data changed since last share |

---

## Print Orders

### Print Service

- Optional feature per studio (`IsPrintServiceEnabled` flag)
- Studio configures available print formats with prices
- Clients select images + format + quantity
- Price snapshotted at order creation

### Print Order Flow

```
1. Client selects images from gallery
2. Chooses format + quantity for each
3. Order created with price snapshots
4. Admin fulfills order
5. ZIP download available for print service
```

---

## Client Management

### Client Data

- Name, email, phone
- Booking history (read model)
- Multi-studio: client may book at different studios

### Privacy Rules

- PII minimized in localStorage
- Client data scoped to studio (multi-tenancy via `StudioId`)
- No email enumeration — auth endpoints return 200 regardless

---

## Studio Configuration

### Configurable Settings (per studio)

| Setting | Purpose |
|---------|---------|
| Operating hours | Day-of-week schedules with time ranges |
| Booking confirmation window | How long before pending expires |
| Buffer minutes | Time between sessions |
| Timezone | `Europe/Chisinau` for Chișinău studio |
| Currency code | `MDL` (Moldovan Leu) |
| Print service enabled | Feature flag for print orders |
| ICS calendar feed | Optional calendar subscription |

### Multi-Tenancy

- Every studio-scoped entity has `StudioId`
- Admin routes: `api/admin/{studioId:guid}/...`
- `StudioScopedActionFilter` validates studio ownership
- Data isolation enforced at repository level

---

## Notification Workflows

### Automated Notifications

| Event | Notification | Timing |
|-------|-------------|--------|
| Booking created | Confirmation email | Immediate |
| Booking confirmed | Confirmation receipt | Immediate |
| Upcoming session | Reminder email | Configurable hours before |
| Gallery shared | Access link email | Admin-triggered |
| Certificate purchased | Delivery email | Immediate |
| Booking cancelled | Cancellation notice | Immediate |

### Notification Rules

1. Every email recorded in outbox — even on immediate success
2. Failure never loses an email — saved as Pending for retry
3. Admin must explicitly trigger sharing — never auto-send
4. Rate limit: 1 resend per 60s per email
5. Social login users auto-confirmed — skip verification email

---

## Time & Scheduling

### Timezone Rules

- Store ALL times in UTC
- Studio timezone: `Europe/Chisinau` (UTC+2 / UTC+3 DST)
- Convert to local time only at display boundary
- Use `TimeZoneInfo.ConvertTimeFromUtc()` before `DayOfWeek` comparisons
- Availability: pre-load all conflicts for entire range, then filter in memory

## Domain Vocabulary

| Term | Meaning |
|------|---------|
| Session | A booked time slot in the studio |
| Resource | A bookable room/space |
| Service | A session type with pricing (e.g., "Solo 30min", "Duo 60min") |
| Add-on | Optional extra for a booking (props, outfits) |
| Gallery | Collection of photos from a session |
| Album | Subdivision within a gallery |
| Certificate | Gift certificate with monetary value |
| Promo code | Discount code for booking |
