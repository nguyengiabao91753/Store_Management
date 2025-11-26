using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers;
[Route("api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductAPIService _productService;
    public ProductController(IProductAPIService productService)
    {
        _productService = productService;
    }

    [HttpGet("check-quantity")]
    public async Task<IActionResult> CheckProductQuantity(int productId, int quantity)
    {
        var rs = await _productService.CheckProductQuantityAsync(productId, quantity);
        if (rs.IsSuccess)
        {
            return Ok(new { Message = "Sufficient quantity available." });
        }
        else
        {
            return BadRequest(new { Message = "Insufficient quantity available." });
        }
    }

    [HttpGet("get-products")]
    public async Task<IActionResult> GetProducts([FromQuery] int? categoryId, [FromQuery] string? q)
    {
        var rs = await _productService.GetProductsAsync(categoryId, q);
        if (rs.IsSuccess)
        {
            return Ok(rs);
        }

        return BadRequest(rs);
    }
}
