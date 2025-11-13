using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Toppings;

namespace CSharpPizza.DTO.Mappings;

public class ToppingProfile : Profile
{
    public ToppingProfile()
    {
        // Topping <-> ToppingDto
        CreateMap<Topping, ToppingDto>();

        CreateMap<ToppingDto, Topping>()
            .ForMember(dest => dest.PizzaToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CartItemToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // CreateToppingDto -> Topping
        CreateMap<CreateToppingDto, Topping>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PizzaToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CartItemToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        // UpdateToppingDto -> Topping
        CreateMap<UpdateToppingDto, Topping>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PizzaToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CartItemToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}