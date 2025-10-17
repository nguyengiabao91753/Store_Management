using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers;
[Route("promotion")]
public class PromotionController : Controller
{
	private readonly IPromotionService _promotionService;

	public PromotionController(IPromotionService promotionService)
    {
		_promotionService = promotionService;
    }

    public async Task<IActionResult> Index()
	{
		var rs = await _promotionService.GetAllPromotionsAsync();
		if (rs.IsSuccess)
		{
			ViewBag.Promotions = rs.Data;
		}
		else
		{
			TempData["err"] = "Lấy danh sách khuyến mãi thất bại: " + rs.Message;
			ViewBag.Promotions = new List<Promotion>();
		}
        return View();
	}

	[HttpGet]
	[Route("create")]
	public IActionResult Create()
	{
		return View("Create");
    }
}
