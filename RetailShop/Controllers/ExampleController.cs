using Microsoft.AspNetCore.Mvc;

namespace RetailShop.Controllers;
[Route("example")]
public class ExampleController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View("Create");
    }
}
