using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Controllers;
[Route("login")]
public class LoginController : Controller
{
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
}
