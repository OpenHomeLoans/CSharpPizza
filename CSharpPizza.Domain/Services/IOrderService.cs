using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Orders;

namespace CSharpPizza.Domain.Services;

public interface IOrderService
{
    Task<List<OrderListDto>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateOrderFromCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus status, CancellationToken cancellationToken = default);
    Task<bool> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}