using RetailShop.Dtos;
using RetailShop.Models;

public interface ICustomerService
{
    Task<ResultService<List<Customer>>> GetAllCustomersAsync();
    Task<ResultService<Customer>> GetCustomerByIdAsync(int id);
    Task<ResultService<Customer>> CreateCustomerAsync(Customer customer);
    Task<ResultService<Customer>> UpdateCustomerAsync(Customer customer);
    Task<ResultService<bool>> DeleteCustomerAsync(int id);
    Task<ResultService<bool>> RestoreCustomerAsync(int id);

    Task<bool> IsEmailOrPhoneExistAsync(string email, string phone);
    Task<bool> IsEmailOrPhoneExistAsync(int customerId, string email, string phone);

    // Tìm kiếm nhiều điều kiện
    Task<ResultService<List<Customer>>> SearchCustomersAsync(
        string? name = null,
        string? phone = null,
        string? email = null,
        DateTime? fromDate = null,
        DateTime? toDate = null
    );
}