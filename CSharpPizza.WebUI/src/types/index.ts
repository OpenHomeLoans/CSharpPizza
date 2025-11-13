// User and Auth types
export interface User {
  id: number;
  name: string;
  email: string;
  mobile: string;
  address: string;
  role: 'Customer' | 'Admin';
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
  id: number;
  name: string;
  price: number;
}

export interface PizzaTopping {
  toppingId: number;
  topping: Topping;
}

export interface Pizza {
  id: number;
  name: string;
  slug: string;
  description: string;
  imageUrl: string;
  basePrice: number;
  isVegetarian: boolean;
  isAvailable: boolean;
  toppings: PizzaTopping[];
}

export interface PizzaList {
  id: number;
  name: string;
  slug: string;
  description: string;
  imageUrl: string;
  basePrice: number;
  isVegetarian: boolean;
  isAvailable: boolean;
}

// Cart types
export interface ToppingCustomization {
  toppingId: number;
  toppingName: string;
  price: number;
}

export interface CartItem {
  id: number;
  pizzaId: number;
  pizzaName: string;
  pizzaImageUrl: string;
  basePrice: number;
  quantity: number;
  customizations: ToppingCustomization[];
  totalPrice: number;
}

export interface Cart {
  id: number;
  userId: number;
  items: CartItem[];
  totalAmount: number;
}

export interface AddToCartRequest {
  pizzaId: number;
  quantity: number;
  toppingIds: number[];
}

export interface UpdateCartItemRequest {
  quantity: number;
  toppingIds: number[];
}

// Order types
export interface OrderItem {
  id: number;
  pizzaId: number;
  pizzaName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  customizations: ToppingCustomization[];
}

export interface Order {
  id: number;
  userId: number;
  orderDate: string;
  status: 'Pending' | 'Confirmed' | 'Preparing' | 'OutForDelivery' | 'Delivered' | 'Cancelled';
  totalAmount: number;
  items: OrderItem[];
}

export interface OrderList {
  id: number;
  orderDate: string;
  status: string;
  totalAmount: number;
  itemCount: number;
}

export interface CreateOrderRequest {
  // Empty for now - order is created from current cart
}