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

    public async Task<ResultService<Category>> GetCategoryByIdAsync(int id)
    {
        var rs = new ResultService<Category>();
        try
        {
            var category = await _db.Categories.Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.CategoryId == id);
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
            rs.Message = $"Error when finding category: {ex.Message}";
        }
        return rs;
    }

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
            _db.Entry(existingCategory).CurrentValues.SetValues(category);
            _db.Categories.Update(existingCategory);
            _db.SaveChanges();
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
}