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
    public OrderController(IOrderAPIService orderAPIService)
    {
        _orderAPIService = orderAPIService;
    }

    //[HttpGet]
    //public async Task<IActionResult> GetOrdersAsync([FromQuery] int userId)
    //{
    //    var orders = await _orderAPIService.GetOrdersByUserIdAsync(userId);
    //    return Ok(orders);
    //}

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderPlaceDto orderPlaceDto)
    {
        var result = await _orderAPIService.PlaceOrderAsync(orderPlaceDto);
        if (result.OrderId!= null)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
