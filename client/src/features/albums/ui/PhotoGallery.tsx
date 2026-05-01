import * as React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { ResponsiveImage } from '@/shared/ui/ResponsiveImage';
import type { ImageDto } from '../model/types';

interface PhotoGalleryProps {
  images: ImageDto[];
  albumTitle?: string;
}

export function PhotoGallery({ images, albumTitle }: PhotoGalleryProps): React.JSX.Element {
  const { t } = useTranslation();
  const [activeIndex, setActiveIndex] = useState<number | null>(null);
  const active = activeIndex === null ? null : images[activeIndex];

  useEffect(() => {
    if (activeIndex === null) return;
    const onKey = (event: KeyboardEvent): void => {
      if (!['Escape', 'ArrowRight', 'ArrowLeft', 'Home', 'End'].includes(event.key)) return;
      event.preventDefault();
      if (event.key === 'Escape') setActiveIndex(null);
      if (event.key === 'ArrowRight') setActiveIndex((i) => (i === null ? i : Math.min(images.length - 1, i + 1)));
      if (event.key === 'ArrowLeft') setActiveIndex((i) => (i === null ? i : Math.max(0, i - 1)));
      if (event.key === 'Home') setActiveIndex(0);
      if (event.key === 'End') setActiveIndex(images.length - 1);
    };
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [activeIndex, images.length]);

  return (
    <>
      <ul className="columns-1 gap-4 space-y-4 sm:columns-2 lg:columns-3">
        {images.map((img, index) => (
          <li key={img.id} className="break-inside-avoid">
            <button
              type="button"
              onClick={() => setActiveIndex(index)}
              className="group block w-full text-left focus:outline-none focus:ring-2 focus:ring-brand"
            >
                <ResponsiveImage
                  src={img.publicUrl}
                  variants={img.variants}
                  alt={albumTitle ? `${albumTitle} ${index + 1}` : ''}
                width={img.width}
                height={img.height}
                sizes="(min-width: 1024px) 33vw, (min-width: 640px) 50vw, 100vw"
                className="w-full"
                imgClassName="h-auto w-full transition-transform duration-700 group-hover:scale-[1.02] motion-reduce:transition-none motion-reduce:transform-none"
              />
            </button>
          </li>
        ))}
      </ul>

      {active && (
        <div
          role="dialog"
          aria-modal="true"
          aria-label={active.originalName}
          className="fixed inset-0 z-50 flex items-center justify-center bg-ink/90 p-4 text-paper"
          onClick={() => setActiveIndex(null)}
        >
          <button
            type="button"
            aria-label="Close gallery"
            className="absolute right-4 top-4 rounded-lg px-3 py-2 text-sm uppercase tracking-[0.2em] focus:outline-none focus:ring-2 focus:ring-brand"
            onClick={() => setActiveIndex(null)}
          >
            {t('common.close')}
          </button>
          <button
            type="button"
            aria-label="Previous image"
            disabled={activeIndex === 0}
            className="absolute left-4 rounded-lg px-3 py-2 text-3xl disabled:opacity-30 focus:outline-none focus:ring-2 focus:ring-brand"
            onClick={(event) => {
              event.stopPropagation();
              setActiveIndex((i) => (i === null ? i : Math.max(0, i - 1)));
            }}
          >
            ‹
          </button>
          <img
            src={active.variants?.full ?? active.publicUrl}
            alt={albumTitle && activeIndex !== null ? `${albumTitle} ${activeIndex + 1}` : ''}
            width={active.width}
            height={active.height}
            className="max-h-[88vh] max-w-[90vw] object-contain"
            onClick={(event) => event.stopPropagation()}
          />
          <button
            type="button"
            aria-label="Next image"
            disabled={activeIndex === images.length - 1}
            className="absolute right-4 rounded-lg px-3 py-2 text-3xl disabled:opacity-30 focus:outline-none focus:ring-2 focus:ring-brand"
            onClick={(event) => {
              event.stopPropagation();
              setActiveIndex((i) => (i === null ? i : Math.min(images.length - 1, i + 1)));
            }}
          >
            ›
          </button>
        </div>
      )}
    </>
  );
}
