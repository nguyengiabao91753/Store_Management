using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;

namespace RetailShop.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _ctx;

        public ProductService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _ctx.Categories
                       .AsNoTracking()
                       .OrderBy(c => c.CategoryName)
                       .ToListAsync();
        }

        public Task<List<Product>> GetProductsAsync(int? categoryId, string? q)
        {
            var query = _ctx.Products.Where(p => p.Active == true)
                            .AsNoTracking();

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.ProductName.Contains(q));

            return query.OrderBy(p => p.ProductName)
                        .ToListAsync();
        }
    }
}
