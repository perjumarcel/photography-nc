import * as React from 'react';
import { cn } from '@/shared/lib/cn';

export interface ResponsiveImageVariants {
  placeholder: string;
  thumbnail: string;
  card: string;
  hero: string;
  full: string;
}

const responsiveWidths = {
  thumbnail: 240,
  card: 640,
  hero: 1600,
  full: 2400,
} as const;

interface ResponsiveImageProps {
  src: string;
  variants?: ResponsiveImageVariants | null;
  alt: string;
  width?: number | null;
  height?: number | null;
  sizes: string;
  className?: string;
  imgClassName?: string;
  loading?: 'eager' | 'lazy';
  fetchPriority?: 'high' | 'low' | 'auto';
}

export function ResponsiveImage({
  src,
  variants,
  alt,
  width,
  height,
  sizes,
  className,
  imgClassName,
  loading = 'lazy',
  fetchPriority = 'auto',
}: ResponsiveImageProps): React.JSX.Element {
  const source = variants?.card ?? src;
  const placeholderStyle = variants?.placeholder ? createPlaceholderStyle(variants.placeholder) : undefined;
  const srcSet = variants
    ? [
      `${variants.thumbnail} ${responsiveWidths.thumbnail}w`,
      `${variants.card} ${responsiveWidths.card}w`,
      `${variants.hero} ${responsiveWidths.hero}w`,
      `${variants.full} ${responsiveWidths.full}w`,
    ].join(', ')
    : undefined;

  return (
    <div
      className={cn('relative overflow-hidden bg-paper-soft', className)}
      style={placeholderStyle}
    >
      <img
        src={source}
        srcSet={srcSet}
        sizes={sizes}
        alt={alt}
        width={width ?? undefined}
        height={height ?? undefined}
        loading={loading}
        decoding="async"
        fetchPriority={fetchPriority}
        className={cn('h-full w-full object-cover', imgClassName)}
      />
    </div>
  );
}

function createPlaceholderStyle(url: string): React.CSSProperties | undefined {
  if (!isSafeImageUrl(url)) return undefined;
  return {
    backgroundImage: `url("${url}")`,
    backgroundSize: 'cover',
    backgroundPosition: 'center',
  };
}

function isSafeImageUrl(url: string): boolean {
  if (/["'()\u0000-\u001f]/.test(url)) return false;
  if (url.startsWith('/')) return true;
  try {
    const parsed = new URL(url);
    return parsed.protocol === 'https:' || parsed.protocol === 'http:';
  } catch {
    return false;
  }
}
