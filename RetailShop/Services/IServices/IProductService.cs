using System.Collections.Generic;
using System.Threading.Tasks;
using RetailShop.Models;

namespace RetailShop.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Product>> SearchAsync(string keyword);
    }
}
