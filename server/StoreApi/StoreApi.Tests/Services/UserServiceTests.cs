using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;
using StoreApi.Services;
using TrickyTrayAPI.DTOs;

namespace StoreApi.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<ITokenService> _tokenMock = new();
    private readonly Mock<IConfiguration> _configMock = new();
    private readonly Mock<IEmailService> _emailMock = new();
    private readonly ILogger<UserService> _logger = new Mock<ILogger<UserService>>().Object;

    private UserService CreateService() =>
        new(_repoMock.Object, _tokenMock.Object, _configMock.Object, _logger, _emailMock.Object);

    private static string HashPassword(string password) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));

    // ── CreateUserAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task CreateUserAsync_NewEmail_ReturnsCreatedUser()
    {
        var dto = new UserCreateDTO { Name = "Alice", Email = "alice@test.com", Password = "password1" };
        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(
            new User { Id = 1, Name = dto.Name, Email = dto.Email, Password = HashPassword(dto.Password), Role = Role.User });

        var service = CreateService();
        var result = await service.CreateUserAsync(dto);

        Assert.Equal(1, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("alice@test.com", result.Email);
    }

    [Fact]
    public async Task CreateUserAsync_DuplicateEmail_ThrowsArgumentException()
    {
        var dto = new UserCreateDTO { Name = "Alice", Email = "exists@test.com", Password = "password1" };
        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(true);

        var service = CreateService();
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateUserAsync(dto));
    }

    // ── AuthenticateAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
    {
        const string password = "mySecret1";
        var user = new User { Id = 5, Name = "Bob", Email = "bob@test.com", Password = HashPassword(password), Role = Role.User };
        _repoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _tokenMock.Setup(t => t.GenerateToken(user.Id, user.Email, user.Name, user.Role)).Returns("jwt-token");

        // GetValue<int> reads via GetSection(key).Value internally
        var expirySection = new Mock<IConfigurationSection>();
        expirySection.Setup(s => s.Value).Returns("60");
        _configMock.Setup(c => c.GetSection("JwtSettings:ExpiryMinutes")).Returns(expirySection.Object);

        var service = CreateService();
        var result = await service.AuthenticateAsync(user.Email, password);

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result!.Token);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Fact]
    public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var service = CreateService();
        var result = await service.AuthenticateAsync("nobody@test.com", "pass");

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPassword_ReturnsNull()
    {
        var user = new User { Id = 1, Email = "a@test.com", Password = HashPassword("correctPass"), Name = "A", Role = Role.User };
        _repoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var service = CreateService();
        var result = await service.AuthenticateAsync(user.Email, "wrongPass");

        Assert.Null(result);
    }

    // ── ForgotPasswordAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task ForgotPasswordAsync_UnknownEmail_DoesNotSendEmail()
    {
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var service = CreateService();
        await service.ForgotPasswordAsync("ghost@test.com");

        _emailMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_KnownEmail_SendsResetEmail()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "alice@test.com", Password = "x", Role = Role.User };
        _repoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
        _configMock.Setup(c => c["ClientBaseUrl"]).Returns("http://localhost:4200");

        var service = CreateService();
        await service.ForgotPasswordAsync(user.Email);

        _emailMock.Verify(e => e.SendPasswordResetEmailAsync(user.Email, user.Name, It.IsAny<string>()), Times.Once);
    }

    // ── ResetPasswordAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_ReturnsTrue()
    {
        var user = new User
        {
            Id = 1, Email = "a@test.com", Name = "A", Password = "old", Role = Role.User,
            ResetPasswordToken = "token123",
            ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1)
        };
        _repoMock.Setup(r => r.GetByResetTokenAsync("token123")).ReturnsAsync(user);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);

        var service = CreateService();
        var result = await service.ResetPasswordAsync("token123", "newPassword");

        Assert.True(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_ExpiredToken_ReturnsFalse()
    {
        var user = new User
        {
            Id = 1, Email = "a@test.com", Name = "A", Password = "old", Role = Role.User,
            ResetPasswordToken = "expired",
            ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(-1)
        };
        _repoMock.Setup(r => r.GetByResetTokenAsync("expired")).ReturnsAsync(user);

        var service = CreateService();
        var result = await service.ResetPasswordAsync("expired", "newPass");

        Assert.False(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_TokenNotFound_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetByResetTokenAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var service = CreateService();
        var result = await service.ResetPasswordAsync("badtoken", "newPass");

        Assert.False(result);
    }
}
