using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices;

public interface IProductService
{
    Task<ResultService<List<Product>>> GetAllProductsAsync();
    Task<ResultService<Product>> GetProductByIdAsync(int id);
    Task<ResultService<Product>> CreateProductAsync(Product product);
    Task<ResultService<Product>> UpdateProductAsync(Product product);
    Task<ResultService<Product>> DeleteProductAsync(int id);
}