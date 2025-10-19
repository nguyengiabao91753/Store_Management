using Microsoft.EntityFrameworkCore;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Data;
using RetailShop.Services.IServices;
public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ResultService<List<Customer>>> GetAllCustomersAsync()
    {
        var customers = await _db.Customers.ToListAsync();
        return ResultService<List<Customer>>.Success(customers);
    }

    public async Task<ResultService<Customer>> GetCustomerByIdAsync(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null)
            return ResultService<Customer>.Fail("Không tìm thấy khách hàng.");

        return ResultService<Customer>.Success(customer);
    }

    public async Task<ResultService<Customer>> CreateCustomerAsync(Customer customer)
    {
        try
        {
            customer.CreatedAt ??= DateTime.Now;
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return ResultService<Customer>.Success(customer, "Thêm khách hàng thành công.");
        }
        catch (Exception ex)
        {
            return ResultService<Customer>.Fail($"Lỗi khi thêm khách hàng: {ex.Message}");
        }
    }

    public async Task<ResultService<Customer>> UpdateCustomerAsync(Customer customer)
    {
        try
        {
            var existing = await _db.Customers.FindAsync(customer.CustomerId);
            if (existing == null)
                return ResultService<Customer>.Fail("Không tìm thấy khách hàng cần cập nhật.");

            existing.Name = customer.Name;
            existing.Phone = customer.Phone;
            existing.Email = customer.Email;
            existing.Address = customer.Address;

            _db.Customers.Update(existing);
            await _db.SaveChangesAsync();

            return ResultService<Customer>.Success(existing, "Cập nhật khách hàng thành công.");
        }
        catch (Exception ex)
        {
            return ResultService<Customer>.Fail($"Lỗi khi cập nhật: {ex.Message}");
        }
    }

    public async Task<ResultService<bool>> DeleteCustomerAsync(int id)
    {
        try
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
                return ResultService<bool>.Fail("Không tìm thấy khách hàng cần xóa.");

            customer.Active = false;
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
            return ResultService<bool>.Success(true, "Xóa khách hàng thành công.");
        }
        catch (Exception ex)
        {
            return ResultService<bool>.Fail($"Lỗi khi xóa: {ex.Message}");
        }
    }
    
     public async Task<ResultService<bool>> RestoreCustomerAsync(int id)
    {
        try
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
                return ResultService<bool>.Fail("Không tìm thấy khách hàng cần xóa.");

            customer.Active = true;
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync(); 
            return ResultService<bool>.Success(true, "Xóa khách hàng thành công.");
        }
        catch (Exception ex)
        {
            return ResultService<bool>.Fail($"Lỗi khi xóa: {ex.Message}");
        }
    }

    public async Task<ResultService<List<Customer>>> SearchCustomersAsync(
        string? name = null,
        string? phone = null,
        string? email = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _db.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(phone))
            query = query.Where(c => c.Phone.Contains(phone));
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(c => c.Email.Contains(email));
        if (fromDate.HasValue)
            query = query.Where(c => c.CreatedAt >= fromDate);
        if (toDate.HasValue)
            query = query.Where(c => c.CreatedAt <= toDate);

        var result = await query.ToListAsync();
        return ResultService<List<Customer>>.Success(result);
    }

    public async Task<bool> IsEmailOrPhoneExistAsync(string email, string phone)
    {
        return await _db.Customers
            .AnyAsync(c => (!string.IsNullOrEmpty(email) && c.Email == email)
                        || (!string.IsNullOrEmpty(phone) && c.Phone == phone));
    }

    public async Task<bool> IsEmailOrPhoneExistAsync(int customerId, string? email, string? phone)
    {
        return await _db.Customers
            .AnyAsync(c =>
                c.CustomerId != customerId &&
                (
                    (!string.IsNullOrEmpty(email) && c.Email == email) ||
                    (!string.IsNullOrEmpty(phone) && c.Phone == phone)
                ));
    }
}