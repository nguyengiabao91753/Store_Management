using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers;
[Route("api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryAPIService _categoryService;

    public CategoryController(ICategoryAPIService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("get-categories")]
    public async Task<IActionResult> GetAllCategory()
    {
        var result = await _categoryService.getAllCategoryAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}