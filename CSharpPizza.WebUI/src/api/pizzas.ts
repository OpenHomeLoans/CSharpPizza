import { apiClient } from './client';
import type { Pizza, PizzaList } from '../types';

export const pizzasApi = {
  getAll: async (): Promise<PizzaList[]> => {
    const response = await apiClient.get<PizzaList[]>('/pizzas');
    return response.data;
  },

  getBySlug: async (slug: string): Promise<Pizza> => {
    const response = await apiClient.get<Pizza>(`/pizzas/${slug}`);
    return response.data;
  },
};