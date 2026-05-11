using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Repositories;
using StoreApi.Services;
using TrickyTrayAPI.DTOs;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponseDto);
    }

    public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToResponseDto(user) : null;
    }

    public async Task<UserResponseDTO> CreateUserAsync(UserCreateDTO createDto)
    {
        if (await _userRepository.EmailExistsAsync(createDto.Email))
        {
            throw new ArgumentException($"Email {createDto.Email} is already registered.");
        }

        var user = new User
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Password = HashPassword(createDto.Password),
            Role = Role.User
        };

        var createdUser = await _userRepository.CreateAsync(user);
        _logger.LogInformation("User created with ID: {UserId}", createdUser.Id);

        return MapToResponseDto(createdUser);
    }

    public async Task<UserResponseDTO?> UpdateUserAsync(int id, UserUpdateDTO updateDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null) return null;

        if (updateDto.Email != null && updateDto.Email != existingUser.Email)
        {
            if (await _userRepository.EmailExistsAsync(updateDto.Email))
            {
                throw new ArgumentException($"Email {updateDto.Email} is already registered.");
            }
            existingUser.Email = updateDto.Email;
        }

        if (updateDto.Name != null) existingUser.Name = updateDto.Name;

        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return updatedUser != null ? MapToResponseDto(updatedUser) : null;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<LoginResponseDTO?> AuthenticateAsync(string email, string password)

    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Login attempt failed: User not found for email {Email}", email);
            return null;
        }

        // Verify password (simplified - in production use proper password verification)
        var hashedPassword = HashPassword(password);
        if (user.Password != hashedPassword)
        {
            _logger.LogWarning("Login attempt failed: Invalid password for email {Email}", email);
            return null;
        }

        var token = _tokenService.GenerateToken(user.Id, user.Email, user.Name, user.Role);
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 60);

        _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

        return new LoginResponseDTO
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresIn = expiryMinutes * 60,
            User = MapToResponseDto(user)
        };
    }

    private static UserResponseDTO MapToResponseDto(User user)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            role = user.Role.ToString()
        };
    }

    private static string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}