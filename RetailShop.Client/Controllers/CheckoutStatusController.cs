using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Client.Controllers
{
    [Route("CheckoutStatus")]
    public class CheckoutStatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
