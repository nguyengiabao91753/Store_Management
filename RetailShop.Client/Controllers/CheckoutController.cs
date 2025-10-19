using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Client.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
