using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<ResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> UserExistsAsync(string username);
    }
}
