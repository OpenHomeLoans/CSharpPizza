import { useQuery } from '@tanstack/react-query';
import { pizzasApi } from '../api';
import { PizzaCard } from '../components/PizzaCard';
import './HomePage.css';

export const HomePage = () => {
  const { data: pizzas, isLoading, error } = useQuery({
    queryKey: ['pizzas'],
    queryFn: pizzasApi.getAll,
  });

  if (isLoading) {
    return (
      <div className="page-container">
        <div className="loading">Loading pizzas...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="page-container">
        <div className="error">Failed to load pizzas. Please try again later.</div>
      </div>
    );
  }

  return (
    <div className="page-container">
      <div className="home-header">
        <h1>Our Delicious Pizzas</h1>
        <p>Choose from our wide selection of handcrafted pizzas</p>
      </div>

      <div className="pizza-grid">
        {pizzas?.map((pizza) => (
          <PizzaCard key={pizza.id} pizza={pizza} />
        ))}
      </div>

      {pizzas?.length === 0 && (
        <div className="empty-state">
          <p>No pizzas available at the moment.</p>
        </div>
      )}
    </div>
  );
};