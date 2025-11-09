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

            string barcode = "PRD-" +
                Convert.ToBase64String(BitConverter.GetBytes(product.ProductId))
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "");

            product.Barcode = barcode;
            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            var inventory = new Inventory
            {
                ProductId = product.ProductId,
                Quantity = product.Quantity, // lấy từ [NotMapped] property
                UpdatedAt = DateTime.Now
            };

            _db.Inventories.Add(inventory);
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
    public async Task<ResultService<List<Product>>> GetAllProductsAsync(bool active = true)
    {
        var rs = new ResultService<List<Product>>();
        try
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventories)
                .Where(p => p.Active == active)
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
                .Include(p => p.Inventories)
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
            var existingProduct = await _db.Products
                .Include(p => p.Inventories)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (existingProduct == null || existingProduct.Active == false)
            {
                rs.IsSuccess = false;
                rs.Message = "Product not found.";
                return rs;
            }

            // Giữ lại các thông tin cũ không được sửa
            product.Barcode = existingProduct.Barcode;
            product.CreatedAt = existingProduct.CreatedAt;
            product.Active = existingProduct.Active;

            // Cập nhật thông tin cơ bản
            _db.Entry(existingProduct).CurrentValues.SetValues(product);
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.SupplierId = product.SupplierId;

            // ===== CẬP NHẬT INVENTORY =====
            var existingInventory = existingProduct.Inventories.FirstOrDefault();
            if (existingInventory != null)
            {
                existingInventory.Quantity = product.Quantity;
                _db.Inventories.Update(existingInventory);
            }
            else
            {
                var newInventory = new Inventory
                {
                    ProductId = existingProduct.ProductId,
                    Quantity = product.Quantity
                };
                _db.Inventories.Add(newInventory);
                existingProduct.Inventories.Add(newInventory);
            }

            await _db.SaveChangesAsync();

            rs.IsSuccess = true;
            rs.Data = existingProduct;
            rs.Message = "Product updated successfully, including inventory quantity.";
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? "";
            rs.IsSuccess = false;
            rs.Message = $"Error updating product: {ex.Message} | Inner: {inner}";
        }
        return rs;
    }


    public async Task<ResultService<Product>> DeleteProductAsync(int id)
    {
        var rs = new ResultService<Product>();
        try
        {
            var existingProduct = await _db.Products.FindAsync(id);
            if (existingProduct == null || existingProduct.Active == false)
            {
                rs.IsSuccess = false;
                rs.Message = "Product not found.";
                return rs;
            }

            existingProduct.Active = false;
            _db.Products.Update(existingProduct);
            _db.SaveChanges();
            rs.IsSuccess = true;
            rs.Data = existingProduct;
            rs.Message = "Product deleted successfully.";
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? "";
            rs.IsSuccess = false;
            rs.Message = $"Error deleting product: {ex.Message} | Inner: {inner}";
        }
        return rs;
    }

    public async Task<ResultService<bool>> RestoreProductAsync(int id)
    {
        var rs = new ResultService<bool>();
        try
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Product not found.";
                rs.Data = false;
                return rs;
            }
            product.Active = true;
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = true;
            rs.Message = "Product deleted successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error deleting product: {ex.Message}";
        }
        return rs;
    }
}
