import { Link } from 'react-router-dom';
import type { PizzaList } from '../types';
import './PizzaCard.css';

interface PizzaCardProps {
  pizza: PizzaList;
}

export const PizzaCard = ({ pizza }: PizzaCardProps) => {
  return (
    <div className="pizza-card">
      <div className="pizza-image">
        <img src={pizza.imageUrl} alt={pizza.name} />
      </div>
      <div className="pizza-content">
        <h3 className="pizza-name">{pizza.name}</h3>
        <p className="pizza-description">{pizza.description}</p>
        <div className="pizza-footer">
          <span className="pizza-price">${pizza.basePrice.toFixed(2)}</span>
          <Link to={`/pizzas/${pizza.slug}`} className="btn-view-details">
            View Details
          </Link>
        </div>
      </div>
    </div>
  );
};