using CSharpPizza.Data.Entities;
using CSharpPizza.DTO.Carts;

namespace CSharpPizza.Domain.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto addToCartDto, CancellationToken cancellationToken = default);
    Task<CartDto> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto updateDto, CancellationToken cancellationToken = default);
    Task<CartDto> RemoveFromCartAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken = default);
    Task<bool> ClearCartAsync(Guid userId, CancellationToken cancellationToken = default);
    decimal ComputeCartTotal(Cart cart);
}