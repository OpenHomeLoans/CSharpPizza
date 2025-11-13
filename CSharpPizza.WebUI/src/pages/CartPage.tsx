import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { cartApi, ordersApi } from '../api';
import { useCartStore } from '../stores/cartStore';
import './CartPage.css';

export const CartPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { fetchCartCount } = useCartStore();

  const { data: cart, isLoading } = useQuery({
    queryKey: ['cart'],
    queryFn: cartApi.get,
  });

  const updateItemMutation = useMutation({
    mutationFn: ({ itemId, quantity, toppingIds }: { itemId: number; quantity: number; toppingIds: number[] }) =>
      cartApi.updateItem(itemId, { quantity, toppingIds }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] });
      fetchCartCount();
      toast.success('Cart updated');
    },
    onError: () => {
      toast.error('Failed to update cart');
    },
  });

  const removeItemMutation = useMutation({
    mutationFn: cartApi.removeItem,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] });
      fetchCartCount();
      toast.success('Item removed from cart');
    },
    onError: () => {
      toast.error('Failed to remove item');
    },
  });

  const createOrderMutation = useMutation({
    mutationFn: ordersApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] });
      fetchCartCount();
      toast.success('Order placed successfully!');
      navigate('/');
    },
    onError: () => {
      toast.error('Failed to create order');
    },
  });

  const handleUpdateQuantity = (itemId: number, currentQuantity: number, delta: number) => {
    const newQuantity = currentQuantity + delta;
    if (newQuantity < 1) return;

    const item = cart?.items.find((i) => i.id === itemId);
    if (!item) return;

    updateItemMutation.mutate({
      itemId,
      quantity: newQuantity,
      toppingIds: item.customizations.map((c) => c.toppingId),
    });
  };

  const handleRemoveItem = (itemId: number) => {
    if (confirm('Remove this item from cart?')) {
      removeItemMutation.mutate(itemId);
    }
  };

  const handleCheckout = () => {
    if (!cart || cart.items.length === 0) {
      toast.error('Cart is empty');
      return;
    }

    if (confirm('Place order?')) {
      createOrderMutation.mutate();
    }
  };

  if (isLoading) {
    return (
      <div className="page-container">
        <div className="loading">Loading cart...</div>
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="page-container">
        <div className="empty-cart">
          <h2>Your cart is empty</h2>
          <p>Add some delicious pizzas to get started!</p>
          <button onClick={() => navigate('/')} className="btn-primary">
            Browse Pizzas
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="page-container">
      <h1>Your Cart</h1>

      <div className="cart-content">
        <div className="cart-items">
          {cart.items.map((item) => (
            <div key={item.id} className="cart-item">
              <img src={item.pizzaImageUrl} alt={item.pizzaName} className="cart-item-image" />
              
              <div className="cart-item-details">
                <h3>{item.pizzaName}</h3>
                <p className="cart-item-base-price">Base: ${item.basePrice.toFixed(2)}</p>
                
                {item.customizations.length > 0 && (
                  <div className="cart-item-toppings">
                    <strong>Toppings:</strong>
                    <ul>
                      {item.customizations.map((topping) => (
                        <li key={topping.toppingId}>
                          {topping.toppingName} (+${topping.price.toFixed(2)})
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>

              <div className="cart-item-actions">
                <div className="quantity-controls">
                  <button
                    onClick={() => handleUpdateQuantity(item.id, item.quantity, -1)}
                    disabled={updateItemMutation.isPending}
                    className="btn-quantity"
                  >
                    -
                  </button>
                  <span className="quantity">{item.quantity}</span>
                  <button
                    onClick={() => handleUpdateQuantity(item.id, item.quantity, 1)}
                    disabled={updateItemMutation.isPending}
                    className="btn-quantity"
                  >
                    +
                  </button>
                </div>

                <p className="cart-item-total">${item.totalPrice.toFixed(2)}</p>

                <button
                  onClick={() => handleRemoveItem(item.id)}
                  disabled={removeItemMutation.isPending}
                  className="btn-remove"
                >
                  Remove
                </button>
              </div>
            </div>
          ))}
        </div>

        <div className="cart-summary">
          <h2>Order Summary</h2>
          <div className="summary-row">
            <span>Items:</span>
            <span>{cart.items.reduce((sum, item) => sum + item.quantity, 0)}</span>
          </div>
          <div className="summary-row total">
            <span>Total:</span>
            <span>${cart.totalAmount.toFixed(2)}</span>
          </div>
          <button
            onClick={handleCheckout}
            disabled={createOrderMutation.isPending}
            className="btn-checkout"
          >
            {createOrderMutation.isPending ? 'Processing...' : 'Checkout'}
          </button>
        </div>
      </div>
    </div>
  );
};