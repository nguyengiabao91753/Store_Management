using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Dtos;
using RetailShop.API.Models;
using RetailShop.API.Services;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers;
[Route("api/paypal")]
[ApiController]
public class PaypalController : ControllerBase
{
    private readonly PaypalService _paypalService;
    private readonly IOrderAPIService _orderService;
    private readonly IPaymentService _paymentService;

    public PaypalController(PaypalService paypalService, IOrderAPIService orderService, IPaymentService paymentService)
    {
        _paypalService = paypalService;
        _orderService = orderService;
        _paymentService = paymentService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] OrderPlaceDto orderPlaceDto)
    {
        var placeOrderResult = await _orderService.PlaceOrderAsync(orderPlaceDto);
        if (!placeOrderResult.IsSuccess || placeOrderResult.Result is not OrderDTO order)
            return BadRequest("Cannot create order");

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var successUrl = $"{baseUrl}/api/paypal/capture?orderId={order.OrderId}";
        var cancelUrl = $"{baseUrl}/checkout/cancel?orderId={order.OrderId}";

        try
        {
            var approvalUrl = await _paypalService.CreatePaymentAsync(order, successUrl, cancelUrl);
            return Ok(new { approval_url = approvalUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("capture")]
    public async Task<IActionResult> CapturePayment([FromQuery] string token, [FromQuery] int orderId)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Missing token");

        try
        {
            var result = await _paypalService.CapturePaymentAsync(token, orderId);
            if (result.Status == "COMPLETED")
            {
                //Thanh toán
                var paymentResult = await _paymentService.CreatePayment(new Payment
                {
                    OrderId = orderId,
                    Amount = 0,
                    PaymentMethod = "e-wallet",
                    PaymentDate = DateTime.UtcNow
                });

                if (paymentResult.IsSuccess == false)
                {
                    return BadRequest(paymentResult);
                }
                // Trả HTML để tự đóng popup và callback về Blazor
                return Content($@"
                        <script>
                            window.opener.postMessage({{ status: 'success', orderId: '{orderId}' }}, '*');
                            window.close();
                        </script>
                    ", "text/html");
            }
            return Content(@"
                    <script>
                        window.opener.postMessage({ status: 'failed' }, '*');
                        window.close();
                    </script>
                ", "text/html");
        }
        catch (Exception ex)
        {
            return BadRequest("Payment failed");
        }
    }
}
