using Microsoft.Extensions.Logging;
using Moq;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;
using StoreApi.Services;

namespace StoreApi.Tests.Services;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _repoMock = new();
    private readonly ILogger<CartService> _logger = new Mock<ILogger<CartService>>().Object;

    private CartService CreateService() => new(_repoMock.Object, _logger);

    // ── AddToCartAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task AddToCartAsync_ValidItem_ReturnsTrue()
    {
        _repoMock.Setup(r => r.AddToCartAsync(It.IsAny<ProductCart>())).ReturnsAsync(true);

        var service = CreateService();
        var result = await service.AddToCartAsync(new CreateCartDto { UserId = 1, GiftId = 2, Count = 3 });

        Assert.True(result);
    }

    [Fact]
    public async Task AddToCartAsync_RepositoryFails_ReturnsFalse()
    {
        _repoMock.Setup(r => r.AddToCartAsync(It.IsAny<ProductCart>())).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.AddToCartAsync(new CreateCartDto { UserId = 1, GiftId = 2, Count = 1 });

        Assert.False(result);
    }

    // ── GetCartByUserIdAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetCartByUserIdAsync_UserHasItems_ReturnsMappedDtos()
    {
        var cartItems = new List<ProductCart>
        {
            new() { Id = 1, UserId = 1, GiftId = 10, Count = 2, Gift = new Gift { Name = "Watch", Picture = "w.jpg", Price = 100 } }
        };
        _repoMock.Setup(r => r.GetCartByUserIdAsync(1)).ReturnsAsync(cartItems);

        var service = CreateService();
        var result = await service.GetCartByUserIdAsync(1);

        Assert.Single(result);
        Assert.Equal("Watch", result[0].GiftName);
        Assert.Equal(100, result[0].Price);
        Assert.Equal(2, result[0].Count);
    }

    [Fact]
    public async Task GetCartByUserIdAsync_EmptyCart_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetCartByUserIdAsync(99)).ReturnsAsync(new List<ProductCart>());

        var service = CreateService();
        var result = await service.GetCartByUserIdAsync(99);

        Assert.Empty(result);
    }

    // ── RemoveFromCartAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task RemoveFromCartAsync_ExistingItem_ReturnsTrue()
    {
        _repoMock.Setup(r => r.RemoveFromCartAsync(5)).ReturnsAsync(true);

        var service = CreateService();
        var result = await service.RemoveFromCartAsync(5);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveFromCartAsync_MissingItem_ReturnsFalse()
    {
        _repoMock.Setup(r => r.RemoveFromCartAsync(99)).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.RemoveFromCartAsync(99);

        Assert.False(result);
    }

    // ── UpdateCartItemAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task UpdateCartItemAsync_ValidItem_ReturnsTrue()
    {
        _repoMock.Setup(r => r.UpdateCartItemAsync(3, 2)).ReturnsAsync(true);

        var service = CreateService();
        var result = await service.UpdateCartItemAsync(3, 2);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateCartItemAsync_MissingItem_ReturnsFalse()
    {
        _repoMock.Setup(r => r.UpdateCartItemAsync(99, 1)).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.UpdateCartItemAsync(99, 1);

        Assert.False(result);
    }
}
