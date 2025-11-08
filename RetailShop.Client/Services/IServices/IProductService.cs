using RetailShop.Client.Models;

namespace RetailShop.Client.Services
{
    public interface IProductService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Product>> GetProductsAsync(int? categoryId, string? q);
    }
}
