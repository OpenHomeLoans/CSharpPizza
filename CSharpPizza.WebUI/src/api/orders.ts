import { apiClient } from './client';
import type { Order, OrderList } from '../types';

export const ordersApi = {
  create: async (): Promise<Order> => {
    const response = await apiClient.post<Order>('/orders');
    return response.data;
  },

  getAll: async (): Promise<OrderList[]> => {
    const response = await apiClient.get<OrderList[]>('/orders');
    return response.data;
  },

  getById: async (id: number): Promise<Order> => {
    const response = await apiClient.get<Order>(`/orders/${id}`);
    return response.data;
  },
};