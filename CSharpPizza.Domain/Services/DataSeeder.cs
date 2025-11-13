using CSharpPizza.Data;
using CSharpPizza.Data.Entities;
using CSharpPizza.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CSharpPizza.Domain.Services;

public class DataSeeder : IDataSeeder
{
    private readonly PizzaDbContext _context;
    private readonly IRepository<Topping> _toppingRepository;
    private readonly IRepository<Pizza> _pizzaRepository;

    public DataSeeder(PizzaDbContext context, IRepository<Topping> toppingRepository, IRepository<Pizza> pizzaRepository)
    {
        _context = context;
        _toppingRepository = toppingRepository;
        _pizzaRepository = pizzaRepository;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Check if database is already seeded
        var existingToppings = await _toppingRepository.GetAllAsync(cancellationToken);
        if (existingToppings.Any())
        {
            return; // Database already seeded
        }

        // Seed toppings
        var toppings = new List<Topping>
        {
            new Topping { Name = "Tomato Sauce", Description = "Classic tomato base sauce", Cost = 0.50m },
            new Topping { Name = "Cheese", Description = "Mozzarella cheese", Cost = 1.50m },
            new Topping { Name = "Ham", Description = "Sliced ham", Cost = 2.00m },
            new Topping { Name = "Pineapple", Description = "Fresh pineapple chunks", Cost = 1.50m },
            new Topping { Name = "BBQ Sauce", Description = "Smoky BBQ sauce", Cost = 0.75m },
            new Topping { Name = "Bacon", Description = "Crispy bacon strips", Cost = 2.50m },
            new Topping { Name = "Sausage", Description = "Italian sausage", Cost = 2.25m },
            new Topping { Name = "Green Bell Pepper", Description = "Fresh green bell peppers", Cost = 1.00m },
            new Topping { Name = "Salsa Sauce", Description = "Spicy salsa sauce", Cost = 0.75m },
            new Topping { Name = "Ground Beef", Description = "Seasoned ground beef", Cost = 2.50m },
            new Topping { Name = "Onions", Description = "Fresh sliced onions", Cost = 0.75m },
            new Topping { Name = "Jalapeno Peppers", Description = "Spicy jalapeno peppers", Cost = 1.25m }
        };

        foreach (var topping in toppings)
        {
            await _toppingRepository.AddAsync(topping, cancellationToken);
        }
        await _toppingRepository.SaveChangesAsync(cancellationToken);

        // Reload toppings to get their IDs
        var seededToppings = (await _toppingRepository.GetAllAsync(cancellationToken)).ToList();

        // Helper function to find topping by name
        Topping GetTopping(string name) => seededToppings.First(t => t.Name == name);

        // Seed pizzas
        var pizzas = new List<Pizza>
        {
            new Pizza
            {
                Name = "Hawaiian Delight",
                Description = "A tropical twist on a classic favorite! Juicy ham and sweet pineapple chunks on a bed of tangy tomato sauce and melted mozzarella cheese. Perfect for those who love a sweet and savory combination.",
                BasePrice = 8.99m,
                Slug = "hawaiian-delight",
                ImageUrl = "https://images.unsplash.com/photo-1534308983496-4fabb1a015ee?w=800&q=80",
                PizzaToppings = new List<PizzaTopping>
                {
                    new PizzaTopping { ToppingId = GetTopping("Tomato Sauce").Id },
                    new PizzaTopping { ToppingId = GetTopping("Cheese").Id },
                    new PizzaTopping { ToppingId = GetTopping("Ham").Id },
                    new PizzaTopping { ToppingId = GetTopping("Pineapple").Id }
                }
            },
            new Pizza
            {
                Name = "Bacon Supreme",
                Description = "A meat lover's dream! Crispy bacon, savory sausage, and melted cheese on a smoky BBQ sauce base, topped with fresh green bell peppers for a perfect balance. This hearty pizza is loaded with flavor in every bite.",
                BasePrice = 11.99m,
                Slug = "bacon-supreme",
                ImageUrl = "https://images.unsplash.com/photo-1565299585323-38d6b0865b47?w=800&q=80",
                PizzaToppings = new List<PizzaTopping>
                {
                    new PizzaTopping { ToppingId = GetTopping("BBQ Sauce").Id },
                    new PizzaTopping { ToppingId = GetTopping("Bacon").Id },
                    new PizzaTopping { ToppingId = GetTopping("Sausage").Id },
                    new PizzaTopping { ToppingId = GetTopping("Cheese").Id },
                    new PizzaTopping { ToppingId = GetTopping("Green Bell Pepper").Id }
                }
            },
            new Pizza
            {
                Name = "Mexican",
                Description = "Spice up your day with this fiesta on a pizza! Seasoned ground beef, fresh onions, and fiery jalapeno peppers on a zesty salsa sauce base. Bold, spicy, and absolutely delicious for those who crave heat.",
                BasePrice = 10.49m,
                Slug = "mexican",
                ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=800&q=80",
                PizzaToppings = new List<PizzaTopping>
                {
                    new PizzaTopping { ToppingId = GetTopping("Salsa Sauce").Id },
                    new PizzaTopping { ToppingId = GetTopping("Ground Beef").Id },
                    new PizzaTopping { ToppingId = GetTopping("Onions").Id },
                    new PizzaTopping { ToppingId = GetTopping("Jalapeno Peppers").Id }
                }
            }
        };

        foreach (var pizza in pizzas)
        {
            await _pizzaRepository.AddAsync(pizza, cancellationToken);
        }
        await _pizzaRepository.SaveChangesAsync(cancellationToken);
    }
}