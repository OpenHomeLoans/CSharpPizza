using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Orders;

namespace CSharpPizza.DTO.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // Order <-> OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderDto, Order>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Status)))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Order -> OrderListDto
        CreateMap<Order, OrderListDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));

        // OrderItem <-> OrderItemDto
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ItemTotal, opt => opt.MapFrom(src => src.PizzaPrice * src.Quantity));

        CreateMap<OrderItemDto, OrderItem>()
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}