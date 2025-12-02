using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<ResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<ResponseDto> CheckUsernameAsync(string username);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        bool IsLoggedIn { get; }
        UserDto? CurrentUser { get; }
        string? Token { get; }
        
            // Event raised when authentication state changes (login/logout)
            event Action? OnAuthStateChanged;
    }
}
