using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Services;

public class ProductAPIService : IProductAPIService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ProductAPIService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<bool> CheckProductQuantityAsync(int productId, int quantity)
    {
        try
        {
            var product = await _db.Products
                                    
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.Active == true);
            if (product == null)
            {
                return false;
            }

            var inventory = await _db.Inventories
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventory == null || inventory.Quantity == 0 || inventory.Quantity < quantity)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }

    public async Task<List<ProductDTO>> GetProductsAsync(int? categoryId, string? q)
    {
        var query = _db.Products.Where(p => p.Active == true)
                            .Include(p => p.Category)
                            .Include(p => p.Supplier)
                            .Include(P => P.Inventories)
                            .AsNoTracking();

        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.ProductName.Contains(q));

        var list = await query.OrderBy(p => p.CreatedAt)
                    .ToListAsync();

        return _mapper.Map<List<ProductDTO>>(list);
    }
}
