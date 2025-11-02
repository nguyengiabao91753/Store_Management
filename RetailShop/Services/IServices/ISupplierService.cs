
using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices;

public interface ISupplierService
{
    Task<ResultService<List<Supplier>>> GetAllSuppliersAsync(bool active = true);
    Task<ResultService<Supplier>> GetSupplierByIdAsync(int id);
    Task<ResultService<Supplier>> CreateSupplierAsync(Supplier supplier);
    Task<ResultService<Supplier>> UpdateSupplierAsync(Supplier supplier);

    Task<ResultService<bool>> DeleteSupplierAsync(int id);

    Task<ResultService<bool>> RestoreSupplierAsync(int id);
}
