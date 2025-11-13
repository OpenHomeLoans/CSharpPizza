using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Carts;

namespace CSharpPizza.DTO.Mappings;

public class CartProfile : Profile
{
    public CartProfile()
    {
        // Cart <-> CartDto
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => 
                src.CartItems.Sum(ci => 
                    (ci.Pizza.BasePrice + ci.CartItemToppings.Sum(cit => cit.Topping.Cost)) * ci.Quantity)));

        CreateMap<CartDto, Cart>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // CartItem <-> CartItemDto
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.PizzaName, opt => opt.MapFrom(src => src.Pizza.Name))
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.Pizza.BasePrice))
            .ForMember(dest => dest.CustomToppings, opt => opt.MapFrom(src => src.CartItemToppings))
            .ForMember(dest => dest.ItemTotal, opt => opt.MapFrom(src => 
                (src.Pizza.BasePrice + src.CartItemToppings.Sum(cit => cit.Topping.Cost)) * src.Quantity));

        CreateMap<CartItemDto, CartItem>()
            .ForMember(dest => dest.CartId, opt => opt.Ignore())
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.Pizza, opt => opt.Ignore())
            .ForMember(dest => dest.CartItemToppings, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // CartItemTopping -> ToppingCustomizationDto
        CreateMap<CartItemTopping, ToppingCustomizationDto>()
            .ForMember(dest => dest.ToppingId, opt => opt.MapFrom(src => src.ToppingId))
            .ForMember(dest => dest.ToppingName, opt => opt.MapFrom(src => src.Topping.Name))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Topping.Cost))
            .ForMember(dest => dest.IsAdded, opt => opt.MapFrom(src => src.IsAdded));
    }
}