using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Client.Controllers
{
    public class CheckoutStatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
