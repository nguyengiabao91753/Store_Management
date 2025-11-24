using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Client.Controllers
{
    [Authorize]
    [Route("CheckoutStatus")]
    public class CheckoutStatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
