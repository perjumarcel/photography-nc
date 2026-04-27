import { jsx as _jsx } from "react/jsx-runtime";
import { cn } from '@/shared/lib/cn';
export function Skeleton({ className }) {
    return _jsx("div", { "aria-hidden": true, className: cn('animate-pulse rounded-md bg-surface-700', className) });
}
