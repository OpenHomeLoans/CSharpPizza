import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import './AdminLayout.css';

// TODO: Should add breadcrumb navigation for better UX

export const AdminLayout = () => {
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="admin-layout">
      <aside className="admin-sidebar">
        <div className="admin-sidebar-header">
          <h2>Admin Portal</h2>
        </div>
        <nav className="admin-nav">
          <Link to="/admin" className="admin-nav-link">
            <span className="nav-icon">📊</span>
            Dashboard
          </Link>
          <Link to="/admin/orders" className="admin-nav-link">
            <span className="nav-icon">📦</span>
            Orders
          </Link>
          <Link to="/admin/pizzas" className="admin-nav-link">
            <span className="nav-icon">🍕</span>
            Pizzas
          </Link>
          <Link to="/admin/toppings" className="admin-nav-link">
            <span className="nav-icon">🧀</span>
            Toppings
          </Link>
        </nav>
      </aside>
      <div className="admin-main">
        <header className="admin-header">
          <div className="admin-header-content">
            <h1>Pizza Management System</h1>
            <div className="admin-user-info">
              <span className="user-name">{user?.name}</span>
              <span className="user-role">{user?.role}</span>
              <button onClick={handleLogout} className="logout-btn">
                Logout
              </button>
            </div>
          </div>
        </header>
        <main className="admin-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
};