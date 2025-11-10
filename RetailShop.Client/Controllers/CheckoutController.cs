using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RetailShop.Client.Dtos;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;
using System.Text;

namespace RetailShop.Client.Controllers
{
    [Authorize]
    [Route("Checkout")]
    public class CheckoutController : Controller
    {
        private readonly IPromotionPOSService _promotionPOSService;
        public CheckoutController(IPromotionPOSService promotionPOSService)
        {
            _promotionPOSService = promotionPOSService;
        }

        public IActionResult Index(string? data)
        {
            List<ProductDto> products = new();

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var json = Encoding.UTF8.GetString(Convert.FromBase64String(data));
                    products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString( json)) ?? new();
                }
                catch { }
            }
            ViewBag.Products = products;
            var order = new OrderPlaceDto
            {
                Products = products,
            };

            return View(order);
        }
    }
}
