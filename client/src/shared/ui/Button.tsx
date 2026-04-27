import * as React from 'react';
import { cn } from '@/shared/lib/cn';

type ButtonVariant = 'primary' | 'ghost' | 'danger';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  loading?: boolean;
}

const variantStyles: Record<ButtonVariant, string> = {
  primary:
    'bg-brand text-brand-text hover:bg-brand-dark focus:ring-brand-light',
  ghost:
    'bg-transparent text-zinc-100 hover:bg-surface-700 focus:ring-zinc-400',
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
        'inline-flex items-center justify-center gap-2 rounded-lg py-2.5 px-4 font-semibold',
        'transition-colors focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:opacity-60',
        variantStyles[variant],
        className,
      )}
      {...rest}
    >
      {loading && (
        <span
          aria-hidden
          className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent"
        />
      )}
      {children}
    </button>
  ),
);
Button.displayName = 'Button';
