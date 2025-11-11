using Microsoft.AspNetCore.Mvc;
using RetailShop.Client.Models;
using RetailShop.Client.Services;

namespace RetailShop.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _svc;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IProductService svc, ILogger<HomeController> logger)
        {
            _svc = svc;
            _logger = logger;
        }


        public async Task<IActionResult> Index(int? categoryId, string? q)
        {
            ViewBag.Categories = await _svc.GetCategoriesAsync();
            ViewBag.ActiveCategoryId = categoryId;
            ViewBag.Query = q;

            var products = await _svc.GetProductsAsync(categoryId, q);
            return View(products); 
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int? categoryId, string? q)
        {
            var products = await _svc.GetProductsAsync(categoryId, q);
            return PartialView("_ProductCards", products);
        }

        [HttpGet("checkAvailable")]
        public async Task<IActionResult> CheckProductQuantity(int productId, int quantity)
        {
            var IsAvailable = await _svc.CheckProductQuantityAsync(productId, quantity);
            return Json(new { isAvailable = IsAvailable });
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
