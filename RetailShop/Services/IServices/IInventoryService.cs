using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices;

public interface IInventoryService
{
    Task<ResultService<List<Inventory>>> GetAllInventoriesAsync();
    Task<ResultService<Inventory>> GetInventoryByIdAsync(int id);
    Task<ResultService<Inventory>> CreateInventoryAsync(Inventory inventory);
    Task<ResultService<Inventory>> UpdateInventoryAsync(Inventory inventory);
}