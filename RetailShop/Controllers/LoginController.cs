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

    [Route("v2")]
    public IActionResult Login()
    {
        return View("Login");
    }
}
