using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Client.Controllers
{
    public class OrderHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
