using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Pizzas;
using CSharpPizza.DTO.Toppings;

namespace CSharpPizza.DTO.Mappings;

public class PizzaProfile : Profile
{
    public PizzaProfile()
    {
        // Pizza <-> PizzaDto
        CreateMap<Pizza, PizzaDto>()
            .ForMember(dest => dest.Toppings, opt => opt.MapFrom(src => 
                src.PizzaToppings.Select(pt => pt.Topping)))
            .ForMember(dest => dest.ComputedCost, opt => opt.MapFrom(src => 
                src.BasePrice + src.PizzaToppings.Sum(pt => pt.Topping.Cost)));

        CreateMap<PizzaDto, Pizza>()
            .ForMember(dest => dest.PizzaToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Pizza -> PizzaListDto
        CreateMap<Pizza, PizzaListDto>();

        // CreatePizzaDto -> Pizza
        CreateMap<CreatePizzaDto, Pizza>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => 
                src.Name.ToLower().Replace(" ", "-")))
            .ForMember(dest => dest.PizzaToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        // UpdatePizzaDto -> Pizza
        CreateMap<UpdatePizzaDto, Pizza>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => 
                src.Name.ToLower().Replace(" ", "-")))
            .ForMember(dest => dest.PizzaToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Topping -> ToppingDto (nested in PizzaDto)
        CreateMap<Topping, ToppingDto>();
    }
}