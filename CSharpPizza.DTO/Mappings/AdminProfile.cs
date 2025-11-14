using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Admin;

namespace CSharpPizza.DTO.Mappings;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        // Order -> AdminOrderListDto
        CreateMap<Order, AdminOrderListDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.User.Email));
    }
}