using CSharpPizza.DTO.Toppings;

namespace CSharpPizza.Domain.Services;

public interface IToppingService
{
    Task<List<ToppingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ToppingDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ToppingDto> CreateAsync(CreateToppingDto createDto, CancellationToken cancellationToken = default);
    Task<ToppingDto> UpdateAsync(Guid id, UpdateToppingDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}