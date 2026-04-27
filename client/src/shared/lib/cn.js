import { clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';
/** Merge Tailwind classes safely. Use everywhere instead of string concatenation. */
export function cn(...inputs) {
    return twMerge(clsx(inputs));
}
