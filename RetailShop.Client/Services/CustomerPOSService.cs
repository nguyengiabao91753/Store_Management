using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Services;

public class CustomerPOSService : ICustomerPOSService
{
    private readonly AppDbContext _db;
    public CustomerPOSService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<Customer?> CheckExistingAsync(string phoneNumber)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Phone == phoneNumber);
        return customer;
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        try
        {
            customer.CreatedAt = DateTime.Now;
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return customer;
        }
        catch (Exception ex)
        {
            return new Customer();
        }
    }

    public async Task<Customer> GetCustomerByPhoneNumberAsync(string phoneNumber)
    {

        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Phone == phoneNumber);
        return customer;
    }
}
