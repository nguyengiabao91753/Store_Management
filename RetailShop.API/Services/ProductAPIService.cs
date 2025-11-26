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

    public async Task<ResponseDto?> CheckProductQuantityAsync(int productId, int quantity)
    {
        var response = new ResponseDto();
        try
        {
            var product = await _db.Products
                                    
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.Active == true);
            if (product == null)
            {
                return response;
            }

            var inventory = await _db.Inventories
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventory == null || inventory.Quantity == 0 || inventory.Quantity < quantity)
            {
                return response;
            }
        }
        catch (Exception ex)
        {
            return response;
        }
        response.IsSuccess = true;

        return response;
    }

    public async Task<ResponseDto?> GetProductsAsync(int? categoryId, string? q)
    {

        var response = new ResponseDto();

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

        response.Result= _mapper.Map<List<ProductDTO>>(list);
        response.IsSuccess = true;
        return response;
    }
}
