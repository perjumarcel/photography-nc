import * as React from 'react';
import { cn } from '@/shared/lib/cn';

interface SectionHeadingProps {
  kicker?: string;
  title: string;
  subtitle?: string;
  align?: 'left' | 'center';
  className?: string;
}

/**
 * Section heading used across pages — mirrors the legacy "h2 + .separator" block.
 */
export function SectionHeading({
  kicker,
  title,
  subtitle,
  align = 'left',
  className,
}: SectionHeadingProps): React.JSX.Element {
  return (
    <header
      className={cn(
        'space-y-2',
        align === 'center' ? 'text-center' : 'text-left',
        className,
      )}
    >
      {kicker && (
        <p className="text-[0.7rem] uppercase tracking-[0.4em] text-ink-muted">{kicker}</p>
      )}
      <h1 className="font-display text-3xl font-light leading-tight text-ink sm:text-4xl md:text-5xl">
        {title}
      </h1>
      <span aria-hidden="true" className={cn('legacy-separator', align === 'center' && 'mx-auto block')} />
      {subtitle && (
        <p className="max-w-2xl text-base text-ink-muted sm:text-lg">{subtitle}</p>
      )}
    </header>
  );
}
