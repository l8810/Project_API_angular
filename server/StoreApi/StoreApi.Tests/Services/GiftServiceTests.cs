using Microsoft.Extensions.Logging;
using Moq;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;
using StoreApi.Services;

namespace StoreApi.Tests.Services;

public class GiftServiceTests
{
    private readonly Mock<IGiftRepository> _repoMock = new();
    private readonly Mock<IEmailService> _emailMock = new();
    private readonly ILogger<GiftService> _logger = new Mock<ILogger<GiftService>>().Object;

    private GiftService CreateService() => new(_repoMock.Object, _emailMock.Object, _logger);

    private static Gift MakeGift(int id = 1, string name = "Test Gift") => new Gift
    {
        Id = id, Name = name, Description = "desc", DonorId = 1, Price = 100,
        CategoryId = 1, Picture = "pic.jpg"
    };

    // ── GetAllGiftsAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllGiftsAsync_ReturnsMappedList()
    {
        _repoMock.Setup(r => r.GetAllGiftsAsync()).ReturnsAsync(new List<Gift> { MakeGift(1), MakeGift(2) });

        var service = CreateService();
        var result = await service.GetAllGiftsAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllGiftsAsync_EmptyRepo_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetAllGiftsAsync()).ReturnsAsync(new List<Gift>());

        var service = CreateService();
        var result = await service.GetAllGiftsAsync();

        Assert.Empty(result);
    }

    // ── GetGiftByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetGiftByIdAsync_ExistingId_ReturnsMappedDto()
    {
        _repoMock.Setup(r => r.GetGiftByIdAsync(1)).ReturnsAsync(MakeGift(1, "My Gift"));

        var service = CreateService();
        var result = await service.GetGiftByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("My Gift", result.Name);
    }

    [Fact]
    public async Task GetGiftByIdAsync_MissingId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetGiftByIdAsync(99)).ReturnsAsync((Gift?)null);

        var service = CreateService();
        var result = await service.GetGiftByIdAsync(99);

        Assert.Null(result);
    }

    // ── CreateGiftAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateGiftAsync_ValidDto_ReturnsCreatedGift()
    {
        var dto = new CreateGiftDto { Name = "New Gift", DonorId = 1, CategoryId = 1, Price = 50 };
        var created = MakeGift(10, "New Gift");
        _repoMock.Setup(r => r.CreateGiftAsync(It.IsAny<Gift>())).ReturnsAsync(created);
        _repoMock.Setup(r => r.GetGiftByIdAsync(10)).ReturnsAsync(created);

        var service = CreateService();
        var result = await service.CreateGiftAsync(dto);

        Assert.Equal(10, result.Id);
        Assert.Equal("New Gift", result.Name);
    }

    // ── DeleteGiftAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteGiftAsync_ExistingId_ReturnsTrue()
    {
        _repoMock.Setup(r => r.DeleteGiftAsync(1)).ReturnsAsync(true);

        var service = CreateService();
        var result = await service.DeleteGiftAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteGiftAsync_MissingId_ReturnsFalse()
    {
        _repoMock.Setup(r => r.DeleteGiftAsync(99)).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.DeleteGiftAsync(99);

        Assert.False(result);
    }

    // ── UpdateGiftAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateGiftAsync_ExistingId_ReturnsMappedDto()
    {
        var dto = new CreateGiftDto { Name = "Updated", DonorId = 1, CategoryId = 1, Price = 200 };
        _repoMock.Setup(r => r.UpdateGiftAsync(1, It.IsAny<Gift>())).ReturnsAsync(MakeGift(1, "Updated"));

        var service = CreateService();
        var result = await service.UpdateGiftAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result!.Name);
    }

    [Fact]
    public async Task UpdateGiftAsync_MissingId_ReturnsNull()
    {
        var dto = new CreateGiftDto { Name = "X", DonorId = 1, CategoryId = 1 };
        _repoMock.Setup(r => r.UpdateGiftAsync(99, It.IsAny<Gift>())).ReturnsAsync((Gift?)null);

        var service = CreateService();
        var result = await service.UpdateGiftAsync(99, dto);

        Assert.Null(result);
    }

    // ── LotteryForGiftAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task LotteryForGiftAsync_HasParticipants_ReturnsWinnerAndSendsEmail()
    {
        var winner = new User { Id = 7, Name = "Winner", Email = "w@test.com", Password = "x", Role = Role.User };
        _repoMock.Setup(r => r.LotteryForGiftAsync(1)).ReturnsAsync(winner);
        _emailMock.Setup(e => e.SendWinnerNotificationAsync(1)).Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.LotteryForGiftAsync(1);

        Assert.NotNull(result);
        Assert.Equal(7, result!.Id);
        Assert.Equal("Winner", result.Name);
        _emailMock.Verify(e => e.SendWinnerNotificationAsync(1), Times.Once);
    }

    [Fact]
    public async Task LotteryForGiftAsync_NoParticipants_ReturnsNull()
    {
        _repoMock.Setup(r => r.LotteryForGiftAsync(5)).ReturnsAsync((User?)null);

        var service = CreateService();
        var result = await service.LotteryForGiftAsync(5);

        Assert.Null(result);
        _emailMock.Verify(e => e.SendWinnerNotificationAsync(It.IsAny<int>()), Times.Never);
    }

    // ── SearchGiftsByNameAsync ───────────────────────────────────────────────

    [Fact]
    public async Task SearchGiftsByNameAsync_MatchingGifts_ReturnsResults()
    {
        _repoMock.Setup(r => r.SearchGiftsByNameAsync("watch")).ReturnsAsync(new List<Gift> { MakeGift(1, "Watch") });

        var service = CreateService();
        var result = await service.SearchGiftsByNameAsync("watch");

        Assert.Single(result);
        Assert.Equal("Watch", result[0].Name);
    }
}
