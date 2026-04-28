import * as React from 'react';
import { cn } from '@/shared/lib/cn';

export function Skeleton({ className }: { className?: string }): React.JSX.Element {
  return <div aria-hidden className={cn('animate-pulse rounded-sm bg-paper-soft', className)} />;
}
