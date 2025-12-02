using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers;
[Route("api/order")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderAPIService _orderAPIService;
    private readonly IPromotionService _promotionService;
    private readonly IInventoryService _inventoryService;

    public OrderController(IOrderAPIService orderAPIService, IPromotionService promotionService, IInventoryService inventoryService)
    {
        _orderAPIService = orderAPIService;
        _promotionService = promotionService;
        _inventoryService = inventoryService;
    }


    //[HttpGet]
    //public async Task<IActionResult> GetOrdersAsync([FromQuery] int userId)
    //{
    //    var orders = await _orderAPIService.GetOrdersByUserIdAsync(userId);
    //    return Ok(orders);
    //}

    [HttpPost("order-place")]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderPlaceDto orderPlaceDto)
    {
        var result = await _orderAPIService.PlaceOrderAsync(orderPlaceDto);
        if (result.IsSuccess)
        {
            if (orderPlaceDto.Products != null)
            {
                foreach (var product in orderPlaceDto.Products)
                {
                    var updateInventory = await _inventoryService.UpdateProductStock(product.ProductId, product.Quantity);
                    if(updateInventory.IsSuccess == false)
                    {
                        return BadRequest(updateInventory);
                    }
                }
            }
            if(orderPlaceDto.PromoId!= null)
            {

                var updatePromors = await _promotionService.UpdatePromotionCount(orderPlaceDto.PromoId!.Value);
                if (updatePromors.IsSuccess == false)
                {
                    return BadRequest(updatePromors);
                }
            }
            return Ok(result);
        }
        return BadRequest(result);
    }
}
