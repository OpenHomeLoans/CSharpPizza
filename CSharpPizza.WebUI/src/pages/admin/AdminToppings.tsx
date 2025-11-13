import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApi } from '../../api';
import type { CreateToppingDto, UpdateToppingDto, Topping } from '../../types';
import './AdminToppings.css';

// TODO: Should add bulk operations (delete multiple, update prices)
// GOTCHA: No confirmation dialog for delete

export const AdminToppings = () => {
  const queryClient = useQueryClient();
  const [editingId, setEditingId] = useState<string | null>(null);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [createFormData, setCreateFormData] = useState<CreateToppingDto>({
    name: '',
    description: '',
    cost: 0,
  });
  const [editFormData, setEditFormData] = useState<UpdateToppingDto>({});

  const { data: toppings, isLoading } = useQuery({
    queryKey: ['admin-toppings'],
    queryFn: () => adminApi.getAllToppings(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateToppingDto) => adminApi.createTopping(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-toppings'] });
      setCreateFormData({ name: '', description: '', cost: 0 });
      setShowCreateForm(false);
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateToppingDto }) =>
      adminApi.updateTopping(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-toppings'] });
      setEditingId(null);
      setEditFormData({});
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => adminApi.deleteTopping(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-toppings'] });
    },
  });

  const handleCreateSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    createMutation.mutate(createFormData);
  };

  const handleEditClick = (topping: Topping) => {
    setEditingId(topping.id);
    setEditFormData({
      name: topping.name,
      description: topping.description,
      cost: topping.cost,
    });
  };

  const handleEditSubmit = (id: string) => {
    updateMutation.mutate({ id: Number(id), data: editFormData });
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setEditFormData({});
  };

  if (isLoading) {
    return (
      <div className="admin-toppings">
        <h2>Toppings Management</h2>
        <div className="loading">Loading toppings...</div>
      </div>
    );
  }

  return (
    <div className="admin-toppings">
      <div className="page-header">
        <h2>Toppings Management</h2>
        <button
          onClick={() => setShowCreateForm(!showCreateForm)}
          className="add-btn"
        >
          {showCreateForm ? 'Cancel' : '+ Add Topping'}
        </button>
      </div>

      {showCreateForm && (
        <div className="create-form-container">
          <h3>Create New Topping</h3>
          <form onSubmit={handleCreateSubmit} className="topping-form">
            <div className="form-row">
              <div className="form-group">
                <label>Name:</label>
                <input
                  type="text"
                  value={createFormData.name}
                  onChange={(e) =>
                    setCreateFormData({ ...createFormData, name: e.target.value })
                  }
                  required
                />
              </div>
              <div className="form-group">
                <label>Cost:</label>
                <input
                  type="number"
                  step="0.01"
                  value={createFormData.cost}
                  onChange={(e) =>
                    setCreateFormData({ ...createFormData, cost: Number(e.target.value) })
                  }
                  required
                />
              </div>
            </div>
            <div className="form-group">
              <label>Description:</label>
              <textarea
                value={createFormData.description}
                onChange={(e) =>
                  setCreateFormData({ ...createFormData, description: e.target.value })
                }
                required
                rows={2}
              />
            </div>
            <div className="form-actions">
              <button type="submit" className="submit-btn">
                Create Topping
              </button>
              <button
                type="button"
                onClick={() => setShowCreateForm(false)}
                className="cancel-btn"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="toppings-table-container">
        <table className="toppings-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
              <th>Cost</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {toppings?.map((topping) => (
              <tr key={topping.id} className={topping.isDeleted ? 'deleted-row' : ''}>
                {editingId === topping.id ? (
                  <>
                    <td>
                      <input
                        type="text"
                        value={editFormData.name || ''}
                        onChange={(e) =>
                          setEditFormData({ ...editFormData, name: e.target.value })
                        }
                        className="edit-input"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        value={editFormData.description || ''}
                        onChange={(e) =>
                          setEditFormData({ ...editFormData, description: e.target.value })
                        }
                        className="edit-input"
                      />
                    </td>
                    <td>
                      <input
                        type="number"
                        step="0.01"
                        value={editFormData.cost || 0}
                        onChange={(e) =>
                          setEditFormData({ ...editFormData, cost: Number(e.target.value) })
                        }
                        className="edit-input cost-input"
                      />
                    </td>
                    <td>
                      {topping.isDeleted ? (
                        <span className="status-badge deleted">Deleted</span>
                      ) : (
                        <span className="status-badge active">Active</span>
                      )}
                    </td>
                    <td>
                      <div className="action-buttons">
                        <button
                          onClick={() => handleEditSubmit(topping.id)}
                          className="save-btn"
                        >
                          Save
                        </button>
                        <button onClick={handleCancelEdit} className="cancel-edit-btn">
                          Cancel
                        </button>
                      </div>
                    </td>
                  </>
                ) : (
                  <>
                    <td className="topping-name">{topping.name}</td>
                    <td className="topping-description">{topping.description}</td>
                    <td className="topping-cost">${topping.cost.toFixed(2)}</td>
                    <td>
                      {topping.isDeleted ? (
                        <span className="status-badge deleted">Deleted</span>
                      ) : (
                        <span className="status-badge active">Active</span>
                      )}
                    </td>
                    <td>
                      <div className="action-buttons">
                        <button
                          onClick={() => handleEditClick(topping)}
                          className="edit-btn"
                          disabled={topping.isDeleted}
                        >
                          Edit
                        </button>
                        <button
                          onClick={() => deleteMutation.mutate(Number(topping.id))}
                          className="delete-btn"
                          disabled={topping.isDeleted}
                        >
                          Delete
                        </button>
                      </div>
                    </td>
                  </>
                )}
              </tr>
            ))}
          </tbody>
        </table>

        {toppings?.length === 0 && (
          <div className="no-data">No toppings found</div>
        )}
      </div>
    </div>
  );
};