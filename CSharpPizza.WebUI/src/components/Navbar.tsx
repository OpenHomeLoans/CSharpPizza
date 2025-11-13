import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';
import './Navbar.css';

export const Navbar = () => {
  const { isAuthenticated, user, logout } = useAuthStore();
  const { itemCount } = useCartStore();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/" className="navbar-brand">
          🍕 CSharp Pizza
        </Link>

        <div className="navbar-links">
          <Link to="/" className="nav-link">
            Home
          </Link>
          
          {isAuthenticated && (
            <Link to="/cart" className="nav-link cart-link">
              Cart
              {itemCount > 0 && (
                <span className="cart-badge">{itemCount}</span>
              )}
            </Link>
          )}

          {isAuthenticated && user?.role === 'Admin' && (
            <Link to="/admin" className="nav-link">
              Admin
            </Link>
          )}

          <div className="navbar-auth">
            {isAuthenticated ? (
              <div className="user-menu">
                <span className="user-name">👤 {user?.name}</span>
                <button onClick={handleLogout} className="btn-logout">
                  Logout
                </button>
              </div>
            ) : (
              <div className="auth-links">
                <Link to="/login" className="btn-link">
                  Login
                </Link>
                <Link to="/register" className="btn-primary">
                  Register
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};