import * as React from 'react';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { PortfolioPage } from '@/pages/PortfolioPage';

export function App(): React.JSX.Element {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/portfolio" replace />} />
        <Route path="/portfolio" element={<PortfolioPage />} />
        {/* Future: /album/:id, /admin/* */}
      </Routes>
    </BrowserRouter>
  );
}
