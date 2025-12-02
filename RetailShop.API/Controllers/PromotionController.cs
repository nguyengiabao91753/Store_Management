using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers;
[Route("api/promotion")]
[ApiController]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _promotionService;
    public PromotionController(IPromotionService productService)
    {
        _promotionService = productService;
    }
    [HttpGet("get-promotion/{code}")]
    public async Task<IActionResult> GetProducts(string code)
    {
        var rs = await _promotionService.GetPromotionByCode(code);
        if (rs.IsSuccess)
        {
            return Ok(rs);
        }

        return BadRequest(rs);
    }
}
