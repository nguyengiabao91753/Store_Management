using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    // === CREATE ===
    public async Task<ResultService<Category>> CreateCategoryAsync(Category category)
    {
        var rs = new ResultService<Category>();
        try
        {
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = category;
            rs.Message = "Category created successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error creating category: {ex.Message}";
        }
        return rs;
    }

    // === READ: Lấy tất cả danh mục ===
    public async Task<ResultService<List<Category>>> GetAllCategoriesAsync()
    {
        var rs = new ResultService<List<Category>>();
        try
        {
            var categories = await _db.Categories.ToListAsync();
            rs.IsSuccess = true;
            rs.Data = categories;
            rs.Message = "Categories retrieved successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving categories: {ex.Message}";
        }
        return rs;
    }

    // === READ: Lấy danh mục theo ID ===
    public async Task<ResultService<Category>> GetCategoryByIdAsync(int id)
    {
        var rs = new ResultService<Category>();
        try
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Category not found.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = category;
                rs.Message = "Category retrieved successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error finding category: {ex.Message}";
        }
        return rs;
    }

    // === UPDATE ===
    public async Task<ResultService<Category>> UpdateCategoryAsync(Category category)
    {
        var rs = new ResultService<Category>();
        try
        {
            var existingCategory = await _db.Categories.FindAsync(category.CategoryId);
            if (existingCategory == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Category not found.";
                return rs;
            }

            existingCategory.CategoryName = category.CategoryName;

            _db.Categories.Update(existingCategory);
            await _db.SaveChangesAsync();

            rs.IsSuccess = true;
            rs.Data = existingCategory;
            rs.Message = "Category updated successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error updating category: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<List<Product>>> GetProductsByCategoryIdAsync(int categoryId)
    {
        var rs = new ResultService<List<Product>>();
        try
        {
            var products = await _db.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Supplier)
                .ToListAsync();

            if (products.Count == 0)
            {
                rs.IsSuccess = false;
                rs.Message = "Không có sản phẩm nào trong danh mục này.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = products;
                rs.Message = "Lấy danh sách sản phẩm thành công.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Lỗi khi lấy sản phẩm theo danh mục: {ex.Message}";
        }
        return rs;
    }

}