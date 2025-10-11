using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices;

public interface ISupplierService
{
    Task<ResultService<List<Supplier>>> GetAllSuppliersAsync();
    Task<ResultService<Supplier>> GetSupplierByIdAsync(int id);
    Task<ResultService<Supplier>> CreateSupplierAsync(Supplier supplier);
    Task<ResultService<Supplier>> UpdateSupplierAsync(Supplier supplier);
}
