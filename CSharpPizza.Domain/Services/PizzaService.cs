using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.Data.Repositories;
using CSharpPizza.Domain.Extensions;
using CSharpPizza.DTO.Pizzas;

namespace CSharpPizza.Domain.Services;

public class PizzaService : IPizzaService
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IRepository<Topping> _toppingRepository;
    private readonly IMapper _mapper;

    public PizzaService(IPizzaRepository pizzaRepository, IRepository<Topping> toppingRepository, IMapper mapper)
    {
        _pizzaRepository = pizzaRepository;
        _toppingRepository = toppingRepository;
        _mapper = mapper;
    }

    public async Task<List<PizzaListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var pizzas = await _pizzaRepository.GetPizzasWithToppingsAsync(cancellationToken);
        var pizzaList = new List<PizzaListDto>();

        foreach (var pizza in pizzas)
        {
            var dto = _mapper.Map<PizzaListDto>(pizza);
            dto.ComputedCost = ComputePizzaCost(pizza);
            pizzaList.Add(dto);
        }

        return pizzaList;
    }

    public async Task<PizzaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pizza = await _pizzaRepository.GetByIdAsync(id, cancellationToken);
        if (pizza == null)
        {
            return null;
        }

        var dto = _mapper.Map<PizzaDto>(pizza);
        dto.ComputedCost = ComputePizzaCost(pizza);
        return dto;
    }

    public async Task<PizzaDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var pizza = await _pizzaRepository.GetBySlugAsync(slug, cancellationToken);
        if (pizza == null)
        {
            return null;
        }

        var dto = _mapper.Map<PizzaDto>(pizza);
        dto.ComputedCost = ComputePizzaCost(pizza);
        return dto;
    }

    public async Task<PizzaDto> CreateAsync(CreatePizzaDto createDto, CancellationToken cancellationToken = default)
    {
 
        var description = createDto.Description;
        if (string.IsNullOrWhiteSpace(description))
        {
            description = await "https://lorem-api.com/api/lorem".FetchDataAsync();
        }

        // Create pizza entity
        var pizza = new Pizza
        {
            Name = createDto.Name,
            Description = description,
            BasePrice = createDto.BasePrice,
            ImageUrl = createDto.ImageUrl,
            Slug = GenerateSlug(createDto.Name)
        };

        // Add pizza toppings
        if (createDto.ToppingIds.Any())
        {
            foreach (var toppingId in createDto.ToppingIds)
            {
                var topping = await _toppingRepository.GetByIdAsync(toppingId, cancellationToken);
                if (topping != null)
                {
                    pizza.PizzaToppings.Add(new PizzaTopping
                    {
                        PizzaId = pizza.Id,
                        ToppingId = toppingId
                    });
                }
            }
        }

        await _pizzaRepository.AddAsync(pizza, cancellationToken);
        await _pizzaRepository.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PizzaDto>(pizza);
        dto.ComputedCost = ComputePizzaCost(pizza);
        return dto;
    }

    public async Task<PizzaDto> UpdateAsync(Guid id, UpdatePizzaDto updateDto, CancellationToken cancellationToken = default)
    {
        var pizza = await _pizzaRepository.GetByIdAsync(id, cancellationToken);
        if (pizza == null)
        {
            throw new KeyNotFoundException($"Pizza with ID {id} not found");
        }

        var description = updateDto.Description;
        if (string.IsNullOrWhiteSpace(description))
        {
            description = await "https://lorem-api.com/api/lorem".FetchDataAsync();
        }

        // Update pizza properties
        pizza.Name = updateDto.Name;
        pizza.Description = description;
        pizza.BasePrice = updateDto.BasePrice;
        pizza.ImageUrl = updateDto.ImageUrl;
        pizza.Slug = GenerateSlug(updateDto.Name);

        // Update toppings
        pizza.PizzaToppings.Clear();
        if (updateDto.ToppingIds.Any())
        {
            foreach (var toppingId in updateDto.ToppingIds)
            {
                var topping = await _toppingRepository.GetByIdAsync(toppingId, cancellationToken);
                if (topping != null)
                {
                    pizza.PizzaToppings.Add(new PizzaTopping
                    {
                        PizzaId = pizza.Id,
                        ToppingId = toppingId
                    });
                }
            }
        }

        await _pizzaRepository.UpdateAsync(pizza, cancellationToken);
        await _pizzaRepository.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PizzaDto>(pizza);
        dto.ComputedCost = ComputePizzaCost(pizza);
        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _pizzaRepository.SoftDeleteAsync(id, cancellationToken);
        if (result)
        {
            await _pizzaRepository.SaveChangesAsync(cancellationToken);
        }
        return result;
    }

    public decimal ComputePizzaCost(Pizza pizza)
    {
        var toppingsCost = pizza.PizzaToppings.Sum(pt => pt.Topping?.Cost ?? 0);
        return pizza.BasePrice + toppingsCost;
    }

    private string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}