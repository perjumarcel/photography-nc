# PostgreSQL schema

All tables live in the default `public` schema. Created and evolved exclusively via EF Core migrations (`dotnet ef migrations add ...`). Never `EnsureCreated()`.

## Tables

### `albums`
| Column           | Type           | Notes                                            |
|------------------|----------------|--------------------------------------------------|
| `id`             | `uuid` PK      | Preserved from legacy data during migration      |
| `title`          | `varchar(64)`  | Required                                         |
| `description`    | `text`         | Nullable                                         |
| `event_date`     | `timestamptz`  | Nullable                                         |
| `client`         | `varchar(128)` | Nullable                                         |
| `location`       | `varchar(128)` | Nullable                                         |
| `show_in_portfolio` | `boolean`   | Indexed                                          |
| `show_in_stories`   | `boolean`   |                                                  |
| `show_in_home`      | `boolean`   | Indexed                                          |
| `category_id`    | `int`          | Soft FK to `categories.id` (no DB-level FK to keep aggregate boundaries clean); Indexed |
| `created_at_utc` | `timestamptz`  |                                                  |
| `updated_at_utc` | `timestamptz`  | Nullable                                         |

### `images`
| Column           | Type           | Notes                                            |
|------------------|----------------|--------------------------------------------------|
| `id`             | `uuid` PK      | Preserved from legacy data                       |
| `album_id`       | `uuid` FK      | `ON DELETE CASCADE`; indexed                     |
| `original_name`  | `varchar(256)` |                                                  |
| `storage_key`    | `varchar(512)` | **Unique** — `albums/{albumId}/images/{imageId}{ext}` |
| `content_type`   | `varchar(128)` |                                                  |
| `size_bytes`     | `bigint`       |                                                  |
| `width`          | `int`          |                                                  |
| `height`         | `int`          |                                                  |
| `orientation`    | `int`          | `0 = vertical`, `1 = horizontal`                 |
| `image_type`     | `int`          | `0 = default`, `1 = cover`                       |
| `checksum`       | `varchar(128)` | Nullable; SHA-256 hex                            |
| `created_at_utc` | `timestamptz`  |                                                  |
| `updated_at_utc` | `timestamptz`  | Nullable                                         |

Composite index `(album_id, image_type)` to speed up cover lookups.

### `categories`
| Column           | Type           | Notes              |
|------------------|----------------|--------------------|
| `id`             | `int` PK       | Identity, preserves legacy IDs where possible |
| `name`           | `varchar(64)`  | Required           |
| `slug`           | `varchar(64)`  | **Unique**         |
| `display_order`  | `int`          |                    |
| `created_at_utc` | `timestamptz`  |                    |
| `updated_at_utc` | `timestamptz`  | Nullable           |

### `tags`
| Column           | Type           | Notes              |
|------------------|----------------|--------------------|
| `id`             | `int` PK       | Identity           |
| `name`           | `varchar(64)`  | **Unique**         |

## Time

All timestamps are stored as UTC (`timestamptz`). Conversion to local time happens at the display boundary in the React client.

## Migrations

```
dotnet ef migrations add Initial \
  --project src/Photography.Infrastructure \
  --startup-project src/Photography.Web

dotnet ef database update \
  --project src/Photography.Infrastructure \
  --startup-project src/Photography.Web
```
