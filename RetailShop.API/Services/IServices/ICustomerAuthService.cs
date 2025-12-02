using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices
{
    public interface ICustomerAuthService
    {
        Task<ResponseDto> RegisterAsync(CustomerRegisterDto registerDto);
        Task<ResponseDto> LoginAsync(CustomerLoginDto loginDto);
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<bool> CustomerExistsAsync(string email);
    }
}
