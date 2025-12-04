using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Dtos;
using RetailShop.API.Models;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers;
[Route("api/order")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderAPIService _orderAPIService;
    private readonly IPromotionService _promotionService;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;

    public OrderController(IOrderAPIService orderAPIService, IPromotionService promotionService, IInventoryService inventoryService, IPaymentService paymentService)
    {
        _orderAPIService = orderAPIService;
        _promotionService = promotionService;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
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
            // Fix: Cast result.Result to Order before accessing OrderId
            var order = result.Result as Order;
            if (order == null)
            {
                return BadRequest("Order creation failed: Result is not an Order.");
            }

            //Thanh toán
            var paymentResult = await _paymentService.CreatePayment(new Payment
            {
                OrderId = order.OrderId,
                Amount = orderPlaceDto.TotalAmount!.Value,
                PaymentMethod = orderPlaceDto.PaymentMethod,
                PaymentDate = DateTime.UtcNow
            });

            if (paymentResult.IsSuccess == false)
            {
                return BadRequest(paymentResult);
            }

            //Cập nhật kho
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
            //Cập nhật Promo
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

    [HttpGet("order-cancle")]
    public async Task<IActionResult> CancleOrderAsync([FromQuery] int orderId)
    {
        var result = await _orderAPIService.CancleOrderAsync(orderId);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("get-orders-by-customer/{cusId}")]
    public async Task<IActionResult> GetOrdersByUserIdAsync(int cusId)
    {
        var result = await _orderAPIService.GetOrdersByCustomer(cusId);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("get-order-by-id/{orderId}")]
    public async Task<IActionResult> GetOrderByIdAsync(int orderId)
    {
        var result = await _orderAPIService.GetOrderById(orderId);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
