import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { PortfolioPage } from '@/pages/PortfolioPage';
export function App() {
    return (_jsx(BrowserRouter, { children: _jsxs(Routes, { children: [_jsx(Route, { path: "/", element: _jsx(Navigate, { to: "/portfolio", replace: true }) }), _jsx(Route, { path: "/portfolio", element: _jsx(PortfolioPage, {}) })] }) }));
}
