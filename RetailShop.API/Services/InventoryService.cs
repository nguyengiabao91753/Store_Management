using Microsoft.EntityFrameworkCore;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;
    public InventoryService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<ResponseDto> UpdateProductStock(int productId, int quantity)
    {
        var rs = new ResponseDto();
        var productInventory = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
        if (productInventory == null)
        {
            rs.IsSuccess = false;
            rs.Message = "Product not found.";
            return rs;
        }
        productInventory.Quantity -= quantity;
        productInventory.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        rs.IsSuccess = true;
        rs.Message = "Product stock updated successfully.";
        rs.Result = productInventory;
        return rs;
    }
}
