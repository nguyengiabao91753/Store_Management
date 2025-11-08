using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Services;

public class InventoryPOSService : IInventoryPOSService
{
    private readonly AppDbContext _db;
    public InventoryPOSService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<string> ReduceStockAsync(int productId, int quantity)
    {
        try
        {
            var product = await _db.Inventories.FirstOrDefaultAsync( i => i.ProductId == productId);
            if (product.Quantity < quantity)
            {
                return "Số lượng mua vượt quá tồn kho";
            }
            product.Quantity -= quantity;
            _db.Inventories.Update(product);
            await _db.SaveChangesAsync();
            return "Cập nhật tồn kho thành công";

        }
        catch (Exception ex)
        {
            return "Lỗi Khi cập nhật tồn kho";

        }
    }
}
