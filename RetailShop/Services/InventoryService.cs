using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;
    public InventoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ResultService<Inventory>> CreateInventoryAsync(Inventory inventory)
    {
        var rs = new ResultService<Inventory>();
        try
        {
            inventory.UpdatedAt = DateTime.Now;
            await _db.Inventories.AddAsync(inventory);
            await _db.SaveChangesAsync();

            rs.IsSuccess = true;
            rs.Data = inventory;
            rs.Message = "Inventory created successfully.";
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? "";
            rs.IsSuccess = false;
            rs.Message = $"Error creating inventory: {ex.Message} | Inner: {inner}";
        }
        return rs;
    }
    public async Task<ResultService<List<Inventory>>> GetAllInventoriesAsync()
    {
        var rs = new ResultService<List<Inventory>>();
        try
        {
            var inventories = await _db.Inventories
                .Include(p => p.Product)
                .AsNoTracking()
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();
            rs.IsSuccess = true;
            rs.Data = inventories;
            rs.Message = "Inventories retrieved successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving inventories: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Inventory>> GetInventoryByIdAsync(int id)
    {
        var rs = new ResultService<Inventory>();
        try
        {
            var inventory = await _db.Inventories
                .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.InventoryId == id);
            if (inventory == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Inventory not found.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = inventory;
                rs.Message = "Inventory retrieved successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error when finding inventory: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Inventory>> UpdateInventoryAsync(Inventory inventory)
    {
        var rs = new ResultService<Inventory>();
        try
        {
            var existingInventory = await _db.Inventories.FindAsync(inventory.InventoryId);
            if (existingInventory == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Supplier not found.";
                return rs;
            }
            _db.Entry(existingInventory).CurrentValues.SetValues(inventory);
            existingInventory.UpdatedAt = DateTime.Now;
            _db.Inventories.Update(existingInventory);
            _db.SaveChanges();
            rs.IsSuccess = true;
            rs.Data = existingInventory;
            rs.Message = "Inventory updated successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error updating inventory: {ex.Message}";
        }
        return rs;
    }
}
