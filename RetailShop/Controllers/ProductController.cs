using Microsoft.AspNetCore.Mvc;
using RetailShop.Services.IServices;
using System.Threading.Tasks;

namespace RetailShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            var products = string.IsNullOrEmpty(keyword)
                ? await _productService.GetAllAsync()
                : await _productService.SearchAsync(keyword);

            ViewBag.Keyword = keyword;
            return View(products);
        }
    }
}
