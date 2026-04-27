import * as React from 'react';
import { cn } from '@/shared/lib/cn';

type ButtonVariant = 'primary' | 'ghost' | 'danger' | 'outline';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  loading?: boolean;
}

const variantStyles: Record<ButtonVariant, string> = {
  primary:
    'bg-ink text-paper hover:bg-ink-soft focus:ring-brand',
  outline:
    'border border-ink text-ink hover:bg-ink hover:text-paper focus:ring-ink',
  ghost:
    'bg-transparent text-ink hover:bg-paper-soft focus:ring-ink',
  danger:
    'bg-red-600 text-white hover:bg-red-700 focus:ring-red-300',
};

/** Standard button. Always renders disabled + spinner during loading. */
export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant = 'primary', loading = false, disabled, children, ...rest }, ref) => (
    <button
      ref={ref}
      disabled={disabled || loading}
      className={cn(
        'inline-flex items-center justify-center gap-2 rounded-none px-6 py-3 text-xs font-medium uppercase tracking-[0.2em]',
        'transition-colors focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:opacity-60',
        variantStyles[variant],
        className,
      )}
      {...rest}
    >
      {loading && (
        <span
          aria-hidden
          className="h-3.5 w-3.5 animate-spin rounded-full border-2 border-current border-t-transparent"
        />
      )}
      {children}
    </button>
  ),
);
Button.displayName = 'Button';
