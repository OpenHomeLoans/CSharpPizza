using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.Data.Repositories;
using CSharpPizza.Domain.Extensions;
using CSharpPizza.DTO.Toppings;

namespace CSharpPizza.Domain.Services;

public class ToppingService : IToppingService
{
    private readonly IRepository<Topping> _toppingRepository;
    private readonly IMapper _mapper;

    public ToppingService(IRepository<Topping> toppingRepository, IMapper mapper)
    {
        _toppingRepository = toppingRepository;
        _mapper = mapper;
    }

    public async Task<List<ToppingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var toppings = await _toppingRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<ToppingDto>>(toppings);
    }

    public async Task<ToppingDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var topping = await _toppingRepository.GetByIdAsync(id, cancellationToken);
        return topping == null ? null : _mapper.Map<ToppingDto>(topping);
    }

    public async Task<ToppingDto> CreateAsync(CreateToppingDto createDto, CancellationToken cancellationToken = default)
    {
    
        var description = createDto.Description;
        if (string.IsNullOrWhiteSpace(description))
        {
            description = await "https://lorem-api.com/api/lorem".FetchDataAsync();
        }

        var topping = new Topping
        {
            Name = createDto.Name,
            Description = description,
            Cost = createDto.Cost
        };

        await _toppingRepository.AddAsync(topping, cancellationToken);
        await _toppingRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ToppingDto>(topping);
    }

    public async Task<ToppingDto> UpdateAsync(Guid id, UpdateToppingDto updateDto, CancellationToken cancellationToken = default)
    {
        var topping = await _toppingRepository.GetByIdAsync(id, cancellationToken);
        if (topping == null)
        {
            throw new KeyNotFoundException($"Topping with ID {id} not found");
        }

        var description = updateDto.Description;
        if (string.IsNullOrWhiteSpace(description))
        {
            description = await "https://lorem-api.com/api/lorem".FetchDataAsync();
        }

        topping.Name = updateDto.Name;
        topping.Description = description;
        topping.Cost = updateDto.Cost;

        await _toppingRepository.UpdateAsync(topping, cancellationToken);
        await _toppingRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ToppingDto>(topping);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _toppingRepository.SoftDeleteAsync(id, cancellationToken);
        if (result)
        {
            await _toppingRepository.SaveChangesAsync(cancellationToken);
        }
        return result;
    }
}