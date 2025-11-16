using RetailShop.Dtos;
using RetailShop.Models;
using System;

namespace RetailShop.Services.IServices;

public interface IProductService
{
    Task<ResultService<List<Product>>> GetAllProductsAsync(bool active = true);
    Task<ResultService<Product>> GetProductByIdAsync(int id);
    Task<ResultService<Product>> CreateProductAsync(Product product);
    Task<ResultService<Product>> UpdateProductAsync(Product product);
    Task<ResultService<Product>> DeleteProductAsync(int id);

    Task<ResultService<bool>> RestoreProductAsync(int id);
}