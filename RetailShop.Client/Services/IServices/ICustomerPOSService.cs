using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices;

public interface ICustomerPOSService
{
    Task<Customer?> CheckExistingAsync(string phoneNumber);

    Task<Customer> CreateCustomerAsync(Customer customer);

    Task<Customer> GetCustomerByPhoneNumberAsync(string phoneNumber);
}
