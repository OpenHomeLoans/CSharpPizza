import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { pizzasApi, cartApi } from '../api';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';
import './PizzaDetailsPage.css';

export const PizzaDetailsPage = () => {
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  const { fetchCartCount } = useCartStore();
  
  const [quantity, setQuantity] = useState(1);
  const [selectedToppings, setSelectedToppings] = useState<number[]>([]);

  const { data: pizza, isLoading, error } = useQuery({
    queryKey: ['pizza', slug],
    queryFn: () => pizzasApi.getBySlug(slug!),
    enabled: !!slug,
  });

  const addToCartMutation = useMutation({
    mutationFn: cartApi.addItem,
    onSuccess: () => {
      toast.success('Pizza added to cart!');
      fetchCartCount();
      navigate('/cart');
    },
    onError: () => {
      toast.error('Failed to add pizza to cart');
    },
  });

  const handleToppingToggle = (toppingId: number) => {
    setSelectedToppings((prev) =>
      prev.includes(toppingId)
        ? prev.filter((id) => id !== toppingId)
        : [...prev, toppingId]
    );
  };

  const handleAddToCart = () => {
    if (!isAuthenticated) {
      toast.error('Please login to add items to cart');
      navigate('/login');
      return;
    }

    if (!pizza) return;

    addToCartMutation.mutate({
      pizzaId: pizza.id,
      quantity,
      toppingIds: selectedToppings,
    });
  };

  const calculateTotalPrice = () => {
    if (!pizza) return 0;
    const toppingsPrice = selectedToppings.reduce((sum, toppingId) => {
      const topping = pizza.toppings.find((t) => t.toppingId === toppingId);
      return sum + (topping?.topping.price || 0);
    }, 0);
    return (pizza.basePrice + toppingsPrice) * quantity;
  };

  if (isLoading) {
    return (
      <div className="page-container">
        <div className="loading">Loading pizza details...</div>
      </div>
    );
  }

  if (error || !pizza) {
    return (
      <div className="page-container">
        <div className="error">Pizza not found</div>
        <button onClick={() => navigate('/')} className="btn-back">
          Back to Home
        </button>
      </div>
    );
  }

  return (
    <div className="page-container">
      <button onClick={() => navigate('/')} className="btn-back">
        ← Back to Home
      </button>

      <div className="pizza-details">
        <div className="pizza-details-image">
          <img src={pizza.imageUrl} alt={pizza.name} />
        </div>

        <div className="pizza-details-content">
          <h1>{pizza.name}</h1>
          {pizza.isVegetarian && <span className="veg-badge">🌱 Vegetarian</span>}
          <p className="pizza-details-description">{pizza.description}</p>
          <p className="pizza-base-price">Base Price: ${pizza.basePrice.toFixed(2)}</p>

          {pizza.toppings.length > 0 && (
            <div className="toppings-section">
              <h3>Customize Your Pizza</h3>
              <div className="toppings-list">
                {pizza.toppings.map((pt) => (
                  <label key={pt.toppingId} className="topping-item">
                    <input
                      type="checkbox"
                      checked={selectedToppings.includes(pt.toppingId)}
                      onChange={() => handleToppingToggle(pt.toppingId)}
                    />
                    <span className="topping-name">{pt.topping.name}</span>
                    <span className="topping-price">+${pt.topping.price.toFixed(2)}</span>
                  </label>
                ))}
              </div>
            </div>
          )}

          <div className="quantity-section">
            <label htmlFor="quantity">Quantity:</label>
            <div className="quantity-controls">
              <button
                onClick={() => setQuantity(Math.max(1, quantity - 1))}
                className="btn-quantity"
              >
                -
              </button>
              <input
                id="quantity"
                type="number"
                min="1"
                value={quantity}
                onChange={(e) => setQuantity(Math.max(1, parseInt(e.target.value) || 1))}
                className="quantity-input"
              />
              <button
                onClick={() => setQuantity(quantity + 1)}
                className="btn-quantity"
              >
                +
              </button>
            </div>
          </div>

          <div className="order-summary">
            <div className="total-price">
              <span>Total:</span>
              <span className="price">${calculateTotalPrice().toFixed(2)}</span>
            </div>
            <button
              onClick={handleAddToCart}
              disabled={addToCartMutation.isPending}
              className="btn-add-to-cart"
            >
              {addToCartMutation.isPending ? 'Adding...' : 'Add to Cart'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};