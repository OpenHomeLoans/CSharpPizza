import { apiClient } from './client';
import type {
  AdminOrderFilter,
  AdminOrderListDto,
  Order,
  CreatePizzaDto,
  UpdatePizzaDto,
  Pizza,
  CreateToppingDto,
  UpdateToppingDto,
  Topping,
} from '../types';

export const adminApi = {
  // Order management
  getOrders: async (filters?: AdminOrderFilter): Promise<AdminOrderListDto[]> => {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status);
    if (filters?.startDate) params.append('startDate', filters.startDate);
    if (filters?.endDate) params.append('endDate', filters.endDate);
    if (filters?.customerName) params.append('customerName', filters.customerName);

    const queryString = params.toString();
    const url = queryString ? `/admin/orders?${queryString}` : '/admin/orders';
    
    const response = await apiClient.get<AdminOrderListDto[]>(url);
    return response.data;
  },

  getOrderById: async (id: number): Promise<Order> => {
    const response = await apiClient.get<Order>(`/admin/orders/${id}`);
    return response.data;
  },

  updateOrderStatus: async (id: number, status: string): Promise<Order> => {
    const response = await apiClient.put<Order>(`/admin/orders/${id}/status`, { status });
    return response.data;
  },

  // Pizza management
  getAllPizzas: async (): Promise<Pizza[]> => {
    const response = await apiClient.get<Pizza[]>('/admin/pizzas');
    return response.data;
  },

  createPizza: async (data: CreatePizzaDto): Promise<Pizza> => {
    const response = await apiClient.post<Pizza>('/admin/pizzas', data);
    return response.data;
  },

  updatePizza: async (id: number, data: UpdatePizzaDto): Promise<Pizza> => {
    const response = await apiClient.put<Pizza>(`/admin/pizzas/${id}`, data);
    return response.data;
  },

  deletePizza: async (id: number): Promise<void> => {
    await apiClient.delete(`/admin/pizzas/${id}`);
  },

  // Topping management
  getAllToppings: async (): Promise<Topping[]> => {
    const response = await apiClient.get<Topping[]>('/admin/toppings');
    return response.data;
  },

  createTopping: async (data: CreateToppingDto): Promise<Topping> => {
    const response = await apiClient.post<Topping>('/admin/toppings', data);
    return response.data;
  },

  updateTopping: async (id: number, data: UpdateToppingDto): Promise<Topping> => {
    const response = await apiClient.put<Topping>(`/admin/toppings/${id}`, data);
    return response.data;
  },

  deleteTopping: async (id: number): Promise<void> => {
    await apiClient.delete(`/admin/toppings/${id}`);
  },
};