import * as React from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { Layout } from '@/shared/ui/Layout';
import { HomePage } from '@/pages/HomePage';
import { PortfolioPage } from '@/pages/PortfolioPage';
import { AlbumDetailPage } from '@/pages/AlbumDetailPage';
import { StoriesPage } from '@/pages/StoriesPage';
import { AboutPage } from '@/pages/AboutPage';
import { ContactPage } from '@/pages/ContactPage';
import { NotFoundPage } from '@/pages/NotFoundPage';
import { LoginPage } from '@/features/auth/ui/LoginPage';
import { RequireAuth } from '@/features/auth/ui/RequireAuth';
import { AdminLayout } from '@/pages/admin/AdminLayout';
import { AdminDashboardPage } from '@/pages/admin/AdminDashboardPage';
import { AdminAlbumsPage } from '@/pages/admin/AdminAlbumsPage';
import { AdminAlbumEditPage } from '@/pages/admin/AdminAlbumEditPage';
import { AdminCategoriesPage } from '@/pages/admin/AdminCategoriesPage';

export function App(): React.JSX.Element {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public site */}
        <Route element={<Layout />}>
          <Route index element={<HomePage />} />
          <Route path="portfolio" element={<PortfolioPage />} />
          <Route path="portfolio/:id" element={<AlbumDetailPage />} />
          <Route path="stories" element={<StoriesPage />} />
          <Route path="about" element={<AboutPage />} />
          <Route path="contact" element={<ContactPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Route>

        {/* Auth */}
        <Route path="/admin/login" element={<LoginPage />} />

        {/* Admin (protected by RequireAuth + backend [Authorize(AdminOnly)]) */}
        <Route
          path="/admin"
          element={(
            <RequireAuth>
              <AdminLayout />
            </RequireAuth>
          )}
        >
          <Route index element={<AdminDashboardPage />} />
          <Route path="albums" element={<AdminAlbumsPage />} />
          <Route path="albums/:id" element={<AdminAlbumEditPage />} />
          <Route path="categories" element={<AdminCategoriesPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
