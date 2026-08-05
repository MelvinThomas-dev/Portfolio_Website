import { Navigate, Outlet, useNavigate } from 'react-router-dom';

export default function AdminRoute() {
  const token = sessionStorage.getItem('adminToken');
  if (!token) return <Navigate to="/admin" replace />;
  return <Outlet />;
}

export function AdminLayout() {
  const navigate = useNavigate();

  const logout = () => {
    sessionStorage.removeItem('adminToken');
    navigate('/admin', { replace: true });
  };

  return (
    <div className="admin-page admin-page--dashboard">
      <header className="admin-header">
        <div className="container admin-header__inner">
          <h1>Analytics Dashboard</h1>
          <button type="button" className="btn btn--outline btn--sm" onClick={logout}>
            Logout
          </button>
        </div>
      </header>
      <main className="admin-main container">
        <Outlet />
      </main>
    </div>
  );
}
