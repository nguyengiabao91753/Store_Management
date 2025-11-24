using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices;

public interface IProductAPIService
{
    Task<List<ProductDTO>> GetProductsAsync(int? categoryId, string? q);

    Task<bool> CheckProductQuantityAsync(int productId, int quantity);
}
