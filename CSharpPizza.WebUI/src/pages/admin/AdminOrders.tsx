import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApi } from '../../api';
import type { AdminOrderFilter } from '../../types';
import './AdminOrders.css';

// TODO: Should implement pagination for large datasets
// TODO: Should add export to CSV functionality

export const AdminOrders = () => {
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState<AdminOrderFilter>({});
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);

  const { data: orders, isLoading } = useQuery({
    queryKey: ['admin-orders', filters],
    queryFn: () => adminApi.getOrders(filters),
  });

  const { data: selectedOrder } = useQuery({
    queryKey: ['admin-order', selectedOrderId],
    queryFn: () => adminApi.getOrderById(Number(selectedOrderId)),
    enabled: !!selectedOrderId,
  });

  const updateStatusMutation = useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) =>
      adminApi.updateOrderStatus(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-orders'] });
      queryClient.invalidateQueries({ queryKey: ['admin-order'] });
    },
  });

  const handleStatusChange = (orderId: string, newStatus: string) => {
    updateStatusMutation.mutate({ id: Number(orderId), status: newStatus });
  };

  const handleFilterChange = (key: keyof AdminOrderFilter, value: string) => {
    setFilters(prev => ({
      ...prev,
      [key]: value || undefined,
    }));
  };

  const clearFilters = () => {
    setFilters({});
  };

  if (isLoading) {
    return (
      <div className="admin-orders">
        <h2>Orders Management</h2>
        <div className="loading">Loading orders...</div>
      </div>
    );
  }

  return (
    <div className="admin-orders">
      <h2>Orders Management</h2>

      <div className="filters-section">
        <div className="filter-group">
          <label>Status:</label>
          <select
            value={filters.status || ''}
            onChange={(e) => handleFilterChange('status', e.target.value)}
          >
            <option value="">All</option>
            <option value="Pending">Pending</option>
            <option value="Confirmed">Confirmed</option>
            <option value="Preparing">Preparing</option>
            <option value="OutForDelivery">Out For Delivery</option>
            <option value="Delivered">Delivered</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>

        <div className="filter-group">
          <label>Start Date:</label>
          <input
            type="date"
            value={filters.startDate || ''}
            onChange={(e) => handleFilterChange('startDate', e.target.value)}
          />
        </div>

        <div className="filter-group">
          <label>End Date:</label>
          <input
            type="date"
            value={filters.endDate || ''}
            onChange={(e) => handleFilterChange('endDate', e.target.value)}
          />
        </div>

        <div className="filter-group">
          <label>Customer Name:</label>
          <input
            type="text"
            placeholder="Search by name..."
            value={filters.customerName || ''}
            onChange={(e) => handleFilterChange('customerName', e.target.value)}
          />
        </div>

        <button onClick={clearFilters} className="clear-filters-btn">
          Clear Filters
        </button>
      </div>

      <div className="orders-table-container">
        <table className="orders-table">
          <thead>
            <tr>
              <th>Order ID</th>
              <th>Customer</th>
              <th>Email</th>
              <th>Date</th>
              <th>Status</th>
              <th>Total</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {orders?.map((order) => (
              <tr key={order.id}>
                <td>#{order.id}</td>
                <td>{order.customerName}</td>
                <td>{order.customerEmail}</td>
                <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                <td>
                  <span className={`status-badge status-${order.status.toLowerCase()}`}>
                    {order.status}
                  </span>
                </td>
                <td>${order.totalAmount.toFixed(2)}</td>
                <td>
                  <button
                    onClick={() => setSelectedOrderId(order.id)}
                    className="view-btn"
                  >
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {orders?.length === 0 && (
          <div className="no-data">No orders found</div>
        )}
      </div>

      {selectedOrder && (
        <div className="order-details-modal" onClick={() => setSelectedOrderId(null)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Order Details - #{selectedOrder.id}</h3>
              <button onClick={() => setSelectedOrderId(null)} className="close-btn">
                ×
              </button>
            </div>
            <div className="modal-body">
              <div className="order-info">
                <p><strong>Status:</strong></p>
                <select
                  value={selectedOrder.status}
                  onChange={(e) => handleStatusChange(selectedOrder.id, e.target.value)}
                  className="status-select"
                >
                  <option value="Pending">Pending</option>
                  <option value="Confirmed">Confirmed</option>
                  <option value="Preparing">Preparing</option>
                  <option value="OutForDelivery">Out For Delivery</option>
                  <option value="Delivered">Delivered</option>
                  <option value="Cancelled">Cancelled</option>
                </select>
              </div>

              <div className="order-items">
                <h4>Order Items:</h4>
                {selectedOrder.items.map((item) => (
                  <div key={item.id} className="order-item">
                    <div className="item-info">
                      <span className="item-name">{item.pizzaName}</span>
                      <span className="item-quantity">x{item.quantity}</span>
                    </div>
                    {item.customToppings.length > 0 && (
                      <div className="item-toppings">
                        Toppings: {item.customToppings.map(t => t.toppingName).join(', ')}
                      </div>
                    )}
                    <div className="item-price">${item.totalPrice.toFixed(2)}</div>
                  </div>
                ))}
              </div>

              <div className="order-total">
                <strong>Total: ${selectedOrder.totalAmount.toFixed(2)}</strong>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};