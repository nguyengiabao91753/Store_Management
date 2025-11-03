namespace RetailShop.Client.Services.IServices;

public interface IInventoryPOSService
{
    Task<string> ReduceStockAsync(int productId, int quantity);
}
