import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { ResponsiveImage } from '@/shared/ui/ResponsiveImage';
import type { ImageDto, LoadStatus } from '@/features/albums/model/types';
import type { UploadQueueItem } from '../lib/useAlbumImageUploadQueue';

interface AdminAlbumImagesProps {
  images: ImageDto[];
  coverImageId?: string | null;
  mutationStatus: LoadStatus;
  uploadQueue: UploadQueueItem[];
  fileInput: React.RefObject<HTMLInputElement | null>;
  onPickFile: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onSetCover: (imageId: string) => void;
  onDelete: (imageId: string) => void;
}

export function AdminAlbumImages({
  images,
  coverImageId,
  mutationStatus,
  uploadQueue,
  fileInput,
  onPickFile,
  onSetCover,
  onDelete,
}: AdminAlbumImagesProps): React.JSX.Element {
  const { t } = useTranslation();

  return (
    <section className="mt-8">
      <header className="flex items-center justify-between">
        <h2 className="font-display text-xl text-ink">{t('admin.images')}</h2>
        <label className="cursor-pointer rounded-lg border border-ink px-4 py-2 text-xs uppercase tracking-[0.2em] text-ink hover:bg-ink hover:text-paper">
          {t('admin.upload')}
          <input
            ref={fileInput}
            type="file"
            accept="image/*"
            multiple
            className="sr-only"
            onChange={onPickFile}
          />
        </label>
      </header>

      {mutationStatus === 'loading' && (
        <p className="mt-3 text-xs text-ink/60">{t('admin.uploading')}</p>
      )}

      {uploadQueue.length > 0 && (
        <ul className="mt-4 space-y-2 rounded-xl border border-zinc-200 bg-paper p-4 text-xs">
          {uploadQueue.map((item) => (
            <li key={item.id} className="flex items-center justify-between gap-4">
              <span className="truncate">{item.name}</span>
              <span className={item.status === 'failed' ? 'text-red-600' : 'text-ink/60'}>
                {item.error ?? item.status}
              </span>
            </li>
          ))}
        </ul>
      )}

      {images.length === 0 ? (
        <p className="mt-6 rounded-xl border border-dashed border-zinc-300 bg-paper p-10 text-center text-ink/60">
          {t('admin.noImages')}
        </p>
      ) : (
        <ul className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {images.map((img) => {
            const isCover = coverImageId === img.id;
            return (
              <li key={img.id} className="overflow-hidden rounded-xl border border-zinc-200 bg-paper">
                <ResponsiveImage
                  src={img.publicUrl}
                  variants={img.variants}
                  alt={img.originalName}
                  width={img.width}
                  height={img.height}
                  sizes="(min-width: 1024px) 33vw, (min-width: 640px) 50vw, 100vw"
                  className="aspect-[4/3] w-full"
                />
                <div className="flex items-center justify-between gap-2 p-3 text-xs">
                  <span className="truncate text-ink/70" title={img.originalName}>{img.originalName}</span>
                  <div className="flex gap-2 whitespace-nowrap">
                    <button
                      type="button"
                      onClick={() => onSetCover(img.id)}
                      disabled={isCover}
                      className="uppercase tracking-[0.18em] text-ink/70 hover:text-ink disabled:cursor-default disabled:text-brand"
                    >
                      {isCover ? '★' : t('admin.cover')}
                    </button>
                    <button
                      type="button"
                      onClick={() => onDelete(img.id)}
                      className="uppercase tracking-[0.18em] text-red-600 hover:text-red-700"
                    >
                      {t('admin.delete')}
                    </button>
                  </div>
                </div>
              </li>
            );
          })}
        </ul>
      )}
    </section>
  );
}
