using Microsoft.AspNetCore.Mvc;
using RetailShop.Client.Dtos;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Controllers
{
    [Route("Order")]
    public class OrderController : Controller
    {
        private readonly IOrderPOSService _orderPOSService;
        private readonly IPaymentPOSService _paymentPOSService;
        private readonly ICustomerPOSService _customerPOSService;
        private readonly IInventoryPOSService _inventoryPOSService;
        private readonly IPromotionPOSService _promotionPOSService;

        public OrderController(IOrderPOSService orderPOSService,
            IPaymentPOSService paymentPOSService,
            ICustomerPOSService customerPOSService,
            IInventoryPOSService inventoryPOSService,
            IPromotionPOSService promotionPOSService)
        {
            _orderPOSService = orderPOSService;
            _paymentPOSService = paymentPOSService;
            _customerPOSService = customerPOSService;
            _inventoryPOSService = inventoryPOSService;
            _promotionPOSService = promotionPOSService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(OrderPlaceDto orderPlaceDto)
        {
            if (orderPlaceDto == null)
                return BadRequest("Empty order");

            // Check customer
            var customerId = 0;
            if (orderPlaceDto.CustomerName != null && orderPlaceDto.CustomerPhone != null)
            {
                var existingCustomer = await _customerPOSService.CheckExistingAsync(orderPlaceDto.CustomerPhone);
                if (existingCustomer != null)
                {
                    customerId = existingCustomer.CustomerId;
                }
                else
                {
                    // Create customer
                    var customer = await _customerPOSService.CreateCustomerAsync(new Customer
                    {
                        Name = orderPlaceDto.CustomerName,
                        Phone = orderPlaceDto.CustomerPhone
                    });
                    if (customer != null)
                    {
                        customerId = customer.CustomerId;
                    }
                    else
                    {
                        return BadRequest("Customer creation failed");
                    }
                }
            }

            //Update inventory
            foreach (var item in orderPlaceDto.Products)
            {
                try
                {

                    await _inventoryPOSService.ReduceStockAsync(item.ProductId, item.Quantity);
                }catch (Exception ex)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Place order
            var order = await _orderPOSService.PlaceOrderAsync(orderPlaceDto, customerId);
            if (order.OrderId == 0)
                return BadRequest("Order placement failed");

          
                var payment = await _paymentPOSService.ProcessPaymentAsync(new Payment
                {
                    OrderId = order.OrderId,
                    Amount = orderPlaceDto.TotalAmount.Value,
                    PaymentMethod = orderPlaceDto.PaymentMethod,
                    PaymentDate = DateTime.Now
                });

            if (payment.PaymentId == 0)
                return BadRequest("Payment processing failed");

           

            // Upodate promotion usage
            if (orderPlaceDto.PromoId != null)
            {
                await _promotionPOSService.UpdateUseCount(orderPlaceDto.PromoId.Value);
            }


            Console.WriteLine("Order received:" + orderPlaceDto);

            return RedirectToAction("Index", "CheckoutStatus");
        }

    }
}
