using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ResultService<Product>> CreateProductAsync(Product product)
    {
        var rs = new ResultService<Product>();
        try
        {
            product.CreatedAt = DateTime.Now;
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = product;
            rs.Message = "Product created successfully.";
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? "";
            rs.IsSuccess = false;
            rs.Message = $"Error creating product: {ex.Message} | Inner: {inner}";
        }
        return rs;
    }
    public async Task<ResultService<List<Product>>> GetAllProductsAsync()
    {
        var rs = new ResultService<List<Product>>();
        try
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            rs.IsSuccess = true;
            rs.Data = products;
            rs.Message = "Products retrieved successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving products: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Product>> GetProductByIdAsync(int id)
    {
        var rs = new ResultService<Product>();
        try
        {
            var product = await _db.Products
                .Include(s => s.Category)
                .Include(s => s.Supplier)
            .FirstOrDefaultAsync(s => s.ProductId == id);
            if (product == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Product not found.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = product;
                rs.Message = "Product retrieved successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error when finding product: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Product>> UpdateProductAsync(Product product)
    {
        var rs = new ResultService<Product>();
        try
        {
            var existingProduct = await _db.Products.FindAsync(product.ProductId);
            if (existingProduct == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Product not found.";
                return rs;
            }
            //existingProduct.ProductName = product.ProductName;
            //existingProduct.Category = product.Category;
            //existingProduct.Supplier = product.Supplier;
            //existingProduct.ProductImage = product.ProductImage;
            //existingProduct.Barcode = product.Barcode;
            //existingProduct.Price = product.Price;
            //existingProduct.Unit = product.Unit;

            product.CreatedAt = existingProduct.CreatedAt;
            _db.Entry(existingProduct).CurrentValues.SetValues(product);
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.SupplierId = product.SupplierId;

            _db.Products.Update(existingProduct);
            _db.SaveChanges();
            rs.IsSuccess = true;
            rs.Data = existingProduct;
            rs.Message = "Product updated successfully.";
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? "";
            rs.IsSuccess = false;
            rs.Message = $"Error creating product: {ex.Message} | Inner: {inner}";
        }
        return rs;
    }
}
