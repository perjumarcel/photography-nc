import * as React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '@/app/hooks';

interface RequireAuthProps {
  children: React.JSX.Element;
  /** Optional role gate; defaults to "any authenticated user". */
  role?: string;
}

/**
 * Route guard. Redirects unauthenticated visitors to `/admin/login`, preserving
 * the originally-requested path in router state so the login page can bounce
 * the user back after a successful sign-in.
 *
 * NOTE: this is a UX guard only — the real authorisation boundary is the
 * backend's `[Authorize(Policy = "AdminOnly")]` attribute.
 */
export function RequireAuth({ children, role }: RequireAuthProps): React.JSX.Element {
  const session = useAppSelector((s) => s.auth.session);
  const location = useLocation();

  if (!session) {
    return <Navigate to="/admin/login" state={{ from: location }} replace />;
  }
  if (role && session.role !== role) {
    return <Navigate to="/" replace />;
  }
  return children;
}
