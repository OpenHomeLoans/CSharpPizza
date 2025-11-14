import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApi } from '../../api';
import type { CreatePizzaDto, UpdatePizzaDto, Pizza, Topping } from '../../types';
import './AdminPizzas.css';

// TODO: Should add image upload functionality
// GOTCHA: No confirmation dialog for delete - easy to accidentally delete

export const AdminPizzas = () => {
  const queryClient = useQueryClient();
  const [showForm, setShowForm] = useState(false);
  const [editingPizza, setEditingPizza] = useState<Pizza | null>(null);
  const [formData, setFormData] = useState<CreatePizzaDto>({
    name: '',
    description: '',
    basePrice: 0,
    imageUrl: '',
    toppingIds: [],
  });

  const { data: pizzas, isLoading: pizzasLoading } = useQuery({
    queryKey: ['admin-pizzas'],
    queryFn: () => adminApi.getAllPizzas(),
  });

  const { data: toppings } = useQuery({
    queryKey: ['admin-toppings'],
    queryFn: () => adminApi.getAllToppings(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreatePizzaDto) => adminApi.createPizza(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-pizzas'] });
      resetForm();
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdatePizzaDto }) =>
      adminApi.updatePizza(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-pizzas'] });
      resetForm();
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => adminApi.deletePizza(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-pizzas'] });
    },
  });

  const resetForm = () => {
    setFormData({
      name: '',
      description: '',
      basePrice: 0,
      imageUrl: '',
      toppingIds: [],
    });
    setEditingPizza(null);
    setShowForm(false);
  };

  const handleEdit = (pizza: Pizza) => {
    setEditingPizza(pizza);
    setFormData({
      name: pizza.name,
      description: pizza.description,
      basePrice: pizza.basePrice,
      imageUrl: pizza.imageUrl || '',
      toppingIds: pizza.toppings.map(t => t.id),
    });
    setShowForm(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingPizza) {
      updateMutation.mutate({ id: Number(editingPizza.id), data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleToppingToggle = (toppingId: string) => {
    setFormData(prev => ({
      ...prev,
      toppingIds: prev.toppingIds.includes(toppingId)
        ? prev.toppingIds.filter(id => id !== toppingId)
        : [...prev.toppingIds, toppingId],
    }));
  };

  if (pizzasLoading) {
    return (
      <div className="admin-pizzas">
        <h2>Pizzas Management</h2>
        <div className="loading">Loading pizzas...</div>
      </div>
    );
  }

  return (
    <div className="admin-pizzas">
      <div className="page-header">
        <h2>Pizzas Management</h2>
        <button onClick={() => setShowForm(!showForm)} className="add-btn">
          {showForm ? 'Cancel' : '+ Add Pizza'}
        </button>
      </div>

      {showForm && (
        <div className="pizza-form-container">
          <h3>{editingPizza ? 'Edit Pizza' : 'Create New Pizza'}</h3>
          <form onSubmit={handleSubmit} className="pizza-form">
            <div className="form-group">
              <label>Name:</label>
              <input
                type="text"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                required
              />
            </div>

            <div className="form-group">
              <label>Description:</label>
              <textarea
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                required
                rows={3}
              />
            </div>

            <div className="form-group">
              <label>Base Price:</label>
              <input
                type="number"
                step="0.01"
                value={formData.basePrice}
                onChange={(e) => setFormData({ ...formData, basePrice: Number(e.target.value) })}
                required
              />
            </div>

            <div className="form-group">
              <label>Image URL:</label>
              <input
                type="text"
                value={formData.imageUrl}
                onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                placeholder="https://example.com/image.jpg"
              />
            </div>

            <div className="form-group">
              <label>Toppings:</label>
              <div className="toppings-checkboxes">
                {toppings?.map((topping: Topping) => (
                  <label key={topping.id} className="checkbox-label">
                    <input
                      type="checkbox"
                      checked={formData.toppingIds.includes(topping.id)}
                      onChange={() => handleToppingToggle(topping.id)}
                    />
                    {topping.name} (+${topping.cost.toFixed(2)})
                  </label>
                ))}
              </div>
            </div>

            <div className="form-actions">
              <button type="submit" className="submit-btn">
                {editingPizza ? 'Update Pizza' : 'Create Pizza'}
              </button>
              <button type="button" onClick={resetForm} className="cancel-btn">
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="pizzas-grid">
        {pizzas?.map((pizza) => (
          <div key={pizza.id} className={`pizza-card ${pizza.isDeleted ? 'deleted' : ''}`}>
            {pizza.imageUrl && (
              <img src={pizza.imageUrl} alt={pizza.name} className="pizza-image" />
            )}
            <div className="pizza-content">
              <h3>{pizza.name}</h3>
              {pizza.isDeleted && <span className="deleted-badge">Deleted</span>}
              <p className="pizza-description">{pizza.description}</p>
              <p className="pizza-price">${pizza.basePrice.toFixed(2)}</p>
              <div className="pizza-toppings">
                <strong>Toppings:</strong>
                <div className="topping-list">
                  {pizza.toppings.map((t: Topping) => (
                    <span key={t.id} className="topping-tag">
                      {t.name}
                    </span>
                  ))}
                </div>
              </div>
              <div className="pizza-actions">
                <button onClick={() => handleEdit(pizza)} className="edit-btn">
                  Edit
                </button>
                <button
                  onClick={() => deleteMutation.mutate(Number(pizza.id))}
                  className="delete-btn"
                  disabled={pizza.isDeleted}
                >
                  Delete
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>

      {pizzas?.length === 0 && (
        <div className="no-data">No pizzas found</div>
      )}
    </div>
  );
};