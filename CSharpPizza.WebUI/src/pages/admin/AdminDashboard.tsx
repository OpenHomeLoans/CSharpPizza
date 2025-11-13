import { useQuery } from '@tanstack/react-query';
import { adminApi } from '../../api';
import './AdminDashboard.css';

// TODO: Should add charts and graphs for better visualization
// GOTCHA: Multiple API calls on mount - no data aggregation endpoint

export const AdminDashboard = () => {
  const { data: orders, isLoading: ordersLoading } = useQuery({
    queryKey: ['admin-orders'],
    queryFn: () => adminApi.getOrders(),
  });

  const { data: pizzas, isLoading: pizzasLoading } = useQuery({
    queryKey: ['admin-pizzas'],
    queryFn: () => adminApi.getAllPizzas(),
  });

  const { data: toppings, isLoading: toppingsLoading } = useQuery({
    queryKey: ['admin-toppings'],
    queryFn: () => adminApi.getAllToppings(),
  });

  const isLoading = ordersLoading || pizzasLoading || toppingsLoading;

  const totalOrders = orders?.length || 0;
  const pendingOrders = orders?.filter(o => o.status === 'Pending').length || 0;
  const totalPizzas = pizzas?.length || 0;
  const totalToppings = toppings?.length || 0;

  if (isLoading) {
    return (
      <div className="admin-dashboard">
        <h2>Dashboard</h2>
        <div className="loading">Loading statistics...</div>
      </div>
    );
  }

  return (
    <div className="admin-dashboard">
      <h2>Dashboard</h2>
      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon">📦</div>
          <div className="stat-content">
            <h3>Total Orders</h3>
            <p className="stat-value">{totalOrders}</p>
          </div>
        </div>
        <div className="stat-card pending">
          <div className="stat-icon">⏳</div>
          <div className="stat-content">
            <h3>Pending Orders</h3>
            <p className="stat-value">{pendingOrders}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">🍕</div>
          <div className="stat-content">
            <h3>Total Pizzas</h3>
            <p className="stat-value">{totalPizzas}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">🧀</div>
          <div className="stat-content">
            <h3>Total Toppings</h3>
            <p className="stat-value">{totalToppings}</p>
          </div>
        </div>
      </div>
    </div>
  );
};