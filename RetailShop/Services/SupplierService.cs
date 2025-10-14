using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _db;
    public SupplierService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ResultService<Supplier>> CreateSupplierAsync(Supplier supplier)
    {
        var rs = new ResultService<Supplier>();
        try
        {
            await _db.Suppliers.AddAsync(supplier);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = supplier;
            rs.Message = "Supplier created successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error creating supplier: {ex.Message}";
        }
        return rs;
    }
    public async Task<ResultService<List<Supplier>>> GetAllSuppliersAsync()
    {
        var rs = new ResultService<List<Supplier>>();
        try
        {
            var suppliers = await _db.Suppliers.ToListAsync();
            rs.IsSuccess = true;
            rs.Data = suppliers;
            rs.Message = "Suppliers retrieved successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving suppliers: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Supplier>> GetSupplierByIdAsync(int id)
    {
        var rs = new ResultService<Supplier>();
        try
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Supplier not found.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = supplier;
                rs.Message = "Supplier retrieved successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error when finding supplier: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Supplier>> UpdateSupplierAsync(Supplier supplier)
    {
        var rs = new ResultService<Supplier>();
        try
        {
            var existingSupplier = await _db.Suppliers.FindAsync(supplier.SupplierId);
            if (existingSupplier == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Supplier not found.";
                return rs;
            }
            existingSupplier.Name = supplier.Name;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Address = supplier.Address;
            _db.Suppliers.Update(existingSupplier);
            _db.SaveChanges();
            rs.IsSuccess = true;
            rs.Data = existingSupplier;
            rs.Message = "Supplier updated successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error updating supplier: {ex.Message}";
        }
        return rs;
    }
}
