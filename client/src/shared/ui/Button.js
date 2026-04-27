import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import * as React from 'react';
import { cn } from '@/shared/lib/cn';
const variantStyles = {
    primary: 'bg-brand text-brand-text hover:bg-brand-dark focus:ring-brand-light',
    ghost: 'bg-transparent text-zinc-100 hover:bg-surface-700 focus:ring-zinc-400',
    danger: 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-300',
};
/** Standard button. Always renders disabled + spinner during loading. */
export const Button = React.forwardRef(({ className, variant = 'primary', loading = false, disabled, children, ...rest }, ref) => (_jsxs("button", { ref: ref, disabled: disabled || loading, className: cn('inline-flex items-center justify-center gap-2 rounded-lg py-2.5 px-4 font-semibold', 'transition-colors focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:opacity-60', variantStyles[variant], className), ...rest, children: [loading && (_jsx("span", { "aria-hidden": true, className: "h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" })), children] })));
Button.displayName = 'Button';
