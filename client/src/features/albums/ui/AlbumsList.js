import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Skeleton } from '@/shared/ui/Skeleton';
import { testIds } from '@/shared/lib/testIds';
/** Pure presenter — receives all data via props, no Redux hooks. */
export function AlbumsList({ albums, status, error, labels }) {
    if (status === 'loading' || status === 'idle') {
        return (_jsxs("div", { "data-testid": testIds.album.list, "aria-busy": "true", className: "grid gap-4 sm:grid-cols-2 lg:grid-cols-3", children: [Array.from({ length: 6 }).map((_, i) => (_jsx(Skeleton, { className: "aspect-[4/3] w-full" }, i))), _jsx("span", { className: "sr-only", children: labels.loading })] }));
    }
    if (status === 'failed') {
        return (_jsx("div", { role: "alert", className: "rounded-xl border border-red-500/30 bg-red-500/10 p-5 text-red-200", children: error ?? labels.error }));
    }
    if (albums.length === 0) {
        return (_jsx("div", { className: "rounded-xl border border-zinc-700 bg-surface-800 p-8 text-center text-zinc-300", children: labels.empty }));
    }
    return (_jsx("ul", { "data-testid": testIds.album.list, className: "grid gap-4 sm:grid-cols-2 lg:grid-cols-3", children: albums.map((album) => (_jsxs("li", { "data-testid": testIds.album.card, className: "rounded-xl border border-zinc-700 bg-surface-800 p-5 transition-colors hover:border-brand/40", children: [_jsx("h3", { className: "text-xl font-bold", children: album.title }), album.location && _jsx("p", { className: "mt-1 text-sm text-zinc-400", children: album.location }), _jsxs("p", { className: "mt-3 text-sm text-zinc-500", children: [album.imageCount, " images"] })] }, album.id))) }));
}
