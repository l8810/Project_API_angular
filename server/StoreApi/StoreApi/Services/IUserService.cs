using StoreApi.DTOs;
using TrickyTrayAPI.DTOs;

public interface IUserService
{
    Task<LoginResponseDTO?> AuthenticateAsync(string email, string password);
    Task<UserResponseDTO> CreateUserAsync(UserCreateDTO createDto);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    Task<UserResponseDTO?> GetUserByIdAsync(int id);
    Task<UserResponseDTO?> UpdateUserAsync(int id, UserUpdateDTO updateDto);
}