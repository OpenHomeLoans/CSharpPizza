// User and Auth types
export interface User {
  id: string;
  name: string;
  email: string;
  mobile: string;
  address: string;
  role: 'Customer' | 'Admin';
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  mobile: string;
  address: string;
}

// Pizza types
export interface Topping {
  id: string;
  name: string;
  description: string;
  cost: number;
  isDeleted?: boolean;
}

export interface Pizza {
  id: string;
  name: string;
  slug: string;
  description: string;
  imageUrl: string;
  basePrice: number;
  computedCost: number;
  toppings: Topping[];
  isDeleted?: boolean;
}

export interface PizzaList {
  id: string;
  name: string;
  slug: string;
  description: string;
  imageUrl: string;
  basePrice: number;
  computedCost: number;
}

// Cart types
export interface ToppingCustomization {
  toppingId: string;
  toppingName: string;
  price: number;
}

export interface CartItem {
  id: string;
  pizzaId: string;
  pizzaName: string;
  basePrice: number;
  quantity: number;
  customToppings: ToppingCustomization[];
  itemTotal: number;
}

export interface Cart {
  id: string;
  userId: string;
  items: CartItem[];
  totalAmount: number;
}

export interface AddToCartRequest {
  pizzaId: string;
  quantity: number;
  addedToppingIds: string[];
  removedToppingIds: string[];
}

export interface UpdateCartItemRequest {
  quantity: number;
  addedToppingIds: string[];
  removedToppingIds: string[];
}

// Order types
export interface OrderItem {
  id: string;
  pizzaId: string;
  pizzaName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  customToppings: ToppingCustomization[];
}

export interface Order {
  id: string;
  userId: string;
  status: 'Pending' | 'Confirmed' | 'Preparing' | 'OutForDelivery' | 'Delivered' | 'Cancelled';
  totalAmount: number;
  items: OrderItem[];
  createdAt: string;
}

export interface OrderList {
  id: string;
  status: string;
  totalAmount: number;
  itemCount: number;
  createdAt: string;
}

export interface CreateOrderRequest {
  // Empty for now - order is created from current cart
}

// Admin types
export interface AdminOrderFilter {
  status?: string;
  startDate?: string;
  endDate?: string;
  customerName?: string;
}

export interface AdminOrderListDto extends OrderList {
  userId: string;
  customerName: string;
  customerEmail: string;
}

// Pizza management types
export interface CreatePizzaDto {
  name: string;
  description: string;
  basePrice: number;
  imageUrl?: string;
  toppingIds: string[];
}

export interface UpdatePizzaDto {
  name: string;
  description: string;
  basePrice: number;
  imageUrl?: string;
  toppingIds: string[];
}

// Topping management types
export interface CreateToppingDto {
  name: string;
  description: string;
  cost: number;
}

export interface UpdateToppingDto {
  name?: string;
  description?: string;
  cost?: number;
}