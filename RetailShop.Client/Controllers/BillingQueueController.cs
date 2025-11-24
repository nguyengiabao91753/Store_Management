using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Client.Controllers
{
    [Authorize]
    public class BillingQueueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
