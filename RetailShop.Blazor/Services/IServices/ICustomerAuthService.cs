using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Services.IServices
{
    public interface ICustomerAuthService
    {
        Task<ResponseDto> RegisterAsync(CustomerRegisterDto registerDto);
        Task<ResponseDto> LoginAsync(CustomerLoginDto loginDto);
        Task<CustomerDto?> GetCustomerProfileAsync();
        Task<bool> IsAuthenticatedAsync();
        Task LogoutAsync();
        Task<string?> GetTokenAsync();
    }
}
