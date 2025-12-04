using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using System.Globalization;           // quan trọng
namespace RetailShop.API.Services;

public class PaypalService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _db;
    private readonly PayPalHttpClient _client;

    public PaypalService(IConfiguration configuration, AppDbContext db)
    {
        _configuration = configuration;
        _db = db;

        var environment = new SandboxEnvironment(_configuration["PayPal:ClientId"], _configuration["PayPal:ClientSecret"]);

        _client = new PayPalHttpClient(environment);
    }

    public async Task<string> CreatePaymentAsync(OrderDTO order, string successUrl, string cancelUrl)
    {
        var orderItems = order.OrderItems ?? new List<OrderItemDTO>();

        if (!orderItems.Any(i => i.Quantity > 0 && i.Price > 0))
        {
            orderItems = new List<OrderItemDTO>
        {
            new OrderItemDTO
            {
                ProductName = $"Order #{order.OrderId} - RetailShop",
                Price = order.TotalAmount ?? 1m,  // Ít nhất 0.01 USD
                Quantity = 1
            }
        };
        }

        decimal totalAmount = orderItems.Sum(x => x.Price * x.Quantity);
        if (totalAmount < 0.01m) throw new InvalidOperationException("Total amount must be at least 0.01 USD.");

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",

            PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                ReferenceId = $"order_{order.OrderId}",
                Description = $"Order #{order.OrderId} - RetailShop",

                // CHỈ GỬI CƠ BẢN: Value + Currency - Không breakdown để tránh lỗi version cũ
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = "USD",
                    Value = totalAmount.ToString("F2", CultureInfo.InvariantCulture)
                    // Không set ItemTotal/Shipping/TaxTotal nếu version không hỗ trợ
                }
            }
        },

            ApplicationContext = new ApplicationContext
            {
                BrandName = "RetailShop",
                LandingPage = "NO_PREFERENCE",
                UserAction = "PAY_NOW",
                ReturnUrl = successUrl,
                CancelUrl = cancelUrl,
                ShippingPreference = "NO_SHIPPING"
            }
        });

        try
        {
            var response = await _client.Execute(request);
            var result = response.Result<Order>();

            var approveLink = result.Links?.FirstOrDefault(x => x.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))?.Href
                              ?? throw new Exception("PayPal did not return approval URL");

            return approveLink;
        }
        catch (PayPalHttp.HttpException ex)
        {
            var debugId = ex.Headers?.GetValues("PayPal-Debug-Id")?.FirstOrDefault() ?? "N/A";
            var statusCode = ex.StatusCode;
            throw new Exception($"PayPal Error {statusCode}: {ex.Message} | Debug ID: {debugId}");
        }
    }

    public async Task<Order> CapturePaymentAsync(string token, int orderId)
    {
        var request = new OrdersCaptureRequest(token);
        request.RequestBody(new OrderActionRequest());

        var response = await _client.Execute(request);
        var result = response.Result<Order>();

        if (result.Status == "COMPLETED")
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = "paid";
                await _db.SaveChangesAsync();
            }
        }

        return result;
    }
}