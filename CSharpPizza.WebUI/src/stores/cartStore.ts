import { create } from 'zustand';
import { cartApi } from '../api';

interface CartState {
  itemCount: number;
  fetchCartCount: () => Promise<void>;
  updateCartCount: (count: number) => void;
}

export const useCartStore = create<CartState>((set) => ({
  itemCount: 0,

  fetchCartCount: async () => {
    try {
      const cart = await cartApi.get();
      const count = cart.items.reduce((sum, item) => sum + item.quantity, 0);
      set({ itemCount: count });
    } catch {
      // If cart fetch fails (e.g., not authenticated), set count to 0
      set({ itemCount: 0 });
    }
  },

  updateCartCount: (count: number) => {
    set({ itemCount: count });
  },
}));