import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { cn } from '@/shared/lib/cn';
import { testIds } from '@/shared/lib/testIds';

export interface CategoryFilterOption {
  /** Stable identifier — number for category id, or `null` for "all". */
  id: number | null;
  label: string;
}

interface CategoryFiltersProps {
  options: CategoryFilterOption[];
  selectedId: number | null;
  onSelect: (id: number | null) => void;
}

/**
 * Inline category filter row. Mirrors the legacy `.gallery-filters` block.
 * Horizontally scrolls on small screens to avoid clipping when many
 * categories are present.
 */
export function CategoryFilters({
  options,
  selectedId,
  onSelect,
}: CategoryFiltersProps): React.JSX.Element {
  const { t } = useTranslation();
  return (
    <nav
      aria-label={t('common.allAlbums')}
      data-testid={testIds.category.list}
      className="-mx-6 mb-8 overflow-x-auto px-6"
    >
      <ul className="flex min-w-max items-center gap-x-6 gap-y-2 text-[0.7rem] uppercase tracking-[0.25em]">
        {options.map((opt) => {
          const active = opt.id === selectedId;
          return (
            <li key={opt.id ?? 'all'}>
              <button
                type="button"
                onClick={() => onSelect(opt.id)}
                aria-pressed={active}
                className={cn(
                  'pb-1 transition-colors',
                  active
                    ? 'border-b border-ink text-ink'
                    : 'border-b border-transparent text-ink-muted hover:text-ink',
                )}
              >
                {opt.label}
              </button>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
