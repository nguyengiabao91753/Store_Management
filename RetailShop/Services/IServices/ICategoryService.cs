using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices;

public interface ICategoryService
{
    Task<ResultService<List<Category>>> GetAllCategoriesAsync();
    Task<ResultService<Category>> GetCategoryByIdAsync(int id);
    Task<ResultService<Category>> CreateCategoryAsync(Category category);
    Task<ResultService<Category>> UpdateCategoryAsync(Category category);
}