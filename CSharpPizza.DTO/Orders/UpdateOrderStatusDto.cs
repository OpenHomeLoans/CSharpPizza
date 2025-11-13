using CSharpPizza.Data.Entities;

namespace CSharpPizza.DTO.Orders;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}