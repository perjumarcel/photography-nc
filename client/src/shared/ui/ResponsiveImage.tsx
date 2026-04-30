import * as React from 'react';
import { cn } from '@/shared/lib/cn';

export interface ResponsiveImageVariants {
  placeholder: string;
  thumbnail: string;
  card: string;
  hero: string;
  full: string;
}

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
  const srcSet = variants
    ? [
      `${variants.thumbnail} 240w`,
      `${variants.card} 640w`,
      `${variants.hero} 1600w`,
      `${variants.full} 2400w`,
    ].join(', ')
    : undefined;

  return (
    <div
      className={cn('relative overflow-hidden bg-paper-soft', className)}
      style={variants?.placeholder ? { backgroundImage: `url("${variants.placeholder}")`, backgroundSize: 'cover', backgroundPosition: 'center' } : undefined}
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
