using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Pizzas;

namespace CSharpPizza.Domain.Services;

public interface IPizzaService
{
    Task<List<PizzaListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PizzaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PizzaDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<PizzaDto> CreateAsync(CreatePizzaDto createDto, CancellationToken cancellationToken = default);
    Task<PizzaDto> UpdateAsync(Guid id, UpdatePizzaDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    decimal ComputePizzaCost(Pizza pizza);
}