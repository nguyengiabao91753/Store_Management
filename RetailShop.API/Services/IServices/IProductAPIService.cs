using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices;

public interface IProductAPIService
{
    Task<ResponseDto?> GetProductsAsync(int? categoryId, string? q);

    Task<ResponseDto?> CheckProductQuantityAsync(int productId, int quantity);
}
