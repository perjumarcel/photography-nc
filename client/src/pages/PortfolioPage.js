import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useTranslation } from 'react-i18next';
import { AlbumsContainer } from '@/features/albums/ui/AlbumsContainer';
export function PortfolioPage() {
    const { t } = useTranslation();
    return (_jsxs("main", { className: "mx-auto max-w-6xl px-6 py-12", children: [_jsx("h1", { className: "mb-8 text-4xl font-bold leading-[1.1] tracking-tight sm:text-5xl", children: t('nav.portfolio') }), _jsx(AlbumsContainer, {})] }));
}
