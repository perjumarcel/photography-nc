import * as React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { Button } from '@/shared/ui/Button';
import {
  createAdminCategory, deleteAdminCategory, fetchAdminCategories, updateAdminCategory,
} from '@/features/admin/api/thunks';
import type { CategoryDto } from '@/features/categories/model/types';

/**
 * Categories admin. Inline create form on top, editable rows below. Each row
 * can be saved (PUT /api/admin/categories/{id}) or deleted (DELETE — the
 * backend refuses with `409 Conflict` if albums still reference the category,
 * which surfaces as a toast in `mutationError`).
 */
export function AdminCategoriesPage(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const { categories, categoriesStatus, mutationStatus, mutationError } = useAppSelector((s) => ({
    categories: s.admin.categories,
    categoriesStatus: s.admin.categoriesStatus,
    mutationStatus: s.admin.mutationStatus,
    mutationError: s.admin.mutationError,
  }));

  useEffect(() => { void dispatch(fetchAdminCategories()); }, [dispatch]);

  return (
    <div>
      <h1 className="font-display text-2xl text-ink">{t('admin.categories')}</h1>

      <CreateForm />

      {mutationError && <p role="alert" className="mt-3 text-sm text-red-600">{mutationError}</p>}

      <div className="mt-6 space-y-3">
        {categoriesStatus === 'loading' && <p className="text-ink/60">{t('common.loading')}</p>}
        {categoriesStatus === 'succeeded' && categories.length === 0 && (
          <p className="text-ink/60">{t('common.empty')}</p>
        )}
        {categories.map((c) => (
          <CategoryRow
            key={c.id}
            category={c}
            disabled={mutationStatus === 'loading'}
            onSave={(dto) => dispatch(updateAdminCategory({ id: c.id, dto }))}
            onDelete={() => {
              if (window.confirm(t('admin.confirmDelete'))) dispatch(deleteAdminCategory(c.id));
            }}
          />
        ))}
      </div>
    </div>
  );
}

function CreateForm(): React.JSX.Element {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const mutationStatus = useAppSelector((s) => s.admin.mutationStatus);
  const [name, setName] = useState('');
  const [slug, setSlug] = useState('');
  const [order, setOrder] = useState<number>(0);

  const onSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();
    if (!name.trim()) return;
    const result = await dispatch(createAdminCategory({
      name: name.trim(),
      slug: slug.trim() || null,
      displayOrder: order,
    }));
    if (createAdminCategory.fulfilled.match(result)) {
      setName(''); setSlug(''); setOrder(0);
      dispatch(fetchAdminCategories());
    }
  };

  return (
    <form onSubmit={onSubmit} className="mt-6 grid gap-3 rounded-xl border border-zinc-200 bg-paper p-5 sm:grid-cols-[1fr_1fr_8rem_auto]">
      <input
        type="text" required placeholder={t('admin.name')} value={name}
        onChange={(e) => setName(e.target.value)}
        className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
      />
      <input
        type="text" placeholder={t('admin.slug')} value={slug}
        onChange={(e) => setSlug(e.target.value)}
        className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
      />
      <input
        type="number" placeholder={t('admin.displayOrder')} value={order}
        onChange={(e) => setOrder(Number(e.target.value))}
        className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
      />
      <Button type="submit" loading={mutationStatus === 'loading'}>{t('admin.newCategory')}</Button>
    </form>
  );
}

interface CategoryRowProps {
  category: CategoryDto;
  disabled: boolean;
  onSave: (dto: { name: string; slug: string | null; displayOrder: number; showAsFilter: boolean }) => void;
  onDelete: () => void;
}

function CategoryRow({ category, disabled, onSave, onDelete }: CategoryRowProps): React.JSX.Element {
  const { t } = useTranslation();
  const [name, setName] = useState(category.name);
  const [slug, setSlug] = useState(category.slug ?? '');
  const [order, setOrder] = useState(category.displayOrder);
  const [showAsFilter, setShowAsFilter] = useState(category.showAsFilter);

  // Re-sync local edits if the row is replaced (e.g. after refetch).
  useEffect(() => {
    setName(category.name);
    setSlug(category.slug ?? '');
    setOrder(category.displayOrder);
    setShowAsFilter(category.showAsFilter);
  }, [category]);

  return (
    <div className="grid gap-3 rounded-xl border border-zinc-200 bg-paper p-4 sm:grid-cols-[1fr_1fr_6rem_auto_auto] sm:items-center">
      <input
        type="text" value={name} onChange={(e) => setName(e.target.value)}
        className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
      />
      <input
        type="text" value={slug} onChange={(e) => setSlug(e.target.value)}
        className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
      />
      <input
        type="number" value={order} onChange={(e) => setOrder(Number(e.target.value))}
        className="rounded-lg border border-ink/20 px-3 py-2 text-ink focus:border-ink focus:outline-none"
      />
      <label className="inline-flex items-center gap-2 text-xs uppercase tracking-[0.18em] text-ink/70">
        <input
          type="checkbox" checked={showAsFilter}
          onChange={(e) => setShowAsFilter(e.target.checked)}
          className="h-4 w-4"
        />
        {t('admin.showAsFilter')}
      </label>
      <div className="flex gap-2 justify-self-end">
        <button
          type="button"
          disabled={disabled}
          onClick={() => onSave({ name: name.trim(), slug: slug.trim() || null, displayOrder: order, showAsFilter })}
          className="text-xs uppercase tracking-[0.18em] text-ink hover:text-brand disabled:opacity-50"
        >
          {t('admin.save')}
        </button>
        <button
          type="button"
          disabled={disabled}
          onClick={onDelete}
          className="text-xs uppercase tracking-[0.18em] text-red-600 hover:text-red-700 disabled:opacity-50"
        >
          {t('admin.delete')}
        </button>
      </div>
    </div>
  );
}
