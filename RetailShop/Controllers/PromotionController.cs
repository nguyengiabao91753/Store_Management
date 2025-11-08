using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services;
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

	public async Task<IActionResult> Index(bool active = true)
	{
		var rs = await _promotionService.GetAllPromotionsAsync(active);
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
	[HttpPost]
	[Route("store")]
	public async Task<IActionResult> Store(Promotion promotion)
	{
		if (!ModelState.IsValid)
		{
			var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
			TempData["err"] = "Thêm thất bại: " + string.Join(", ", errors);
			return View("Create", promotion);
		}
		var result = await _promotionService.CreatePromotionAsync(promotion);
		if (result.IsSuccess)
		{
			TempData["success"] = "Thêm thành công";
			return RedirectToAction("Index");
		}
		else
		{
			TempData["err"] = "Thêm thất bại: " + result.Message;
			return View("Create", promotion);
		}
	}

	[HttpGet]
	[Route("edit/{id}")]
	public async Task<IActionResult> Edit(int id)
	{
		var rs = await _promotionService.GetPromotionByIdAsync(id);
		if (rs.IsSuccess)
		{
			return View("Edit", rs.Data);
		}
		TempData["err"] = "Lấy mã giảm giá thất bại" + rs.Message;
		return RedirectToAction("Index");
	}

	[HttpPost]
	[Route("update")]
	public async Task<IActionResult> Update(Promotion promotion)
	{
		if (!ModelState.IsValid)
		{
			var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
			TempData["err"] = "Cập nhật thất bại: " + string.Join(", ", errors);
			return View("Edit", promotion);
		}
		var result = await _promotionService.UpdatePromotionAsync(promotion);
		if (result.IsSuccess)
		{
			TempData["success"] = "Cập nhật thành công";
			return RedirectToAction("Index");
		}
		else
		{
			TempData["err"] = "Cập nhật thất bại: " + result.Message;
			return View("Edit", promotion);
		}
	}


	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("delete/{id}")]
	public async Task<IActionResult> DeletePost(int id)
	{
		var result = await _promotionService.DeletePromotionAsync(id);
		if (result.IsSuccess)
		{
			TempData["success"] = result.Message;
		}
		else
		{
			TempData["err"] = "Xóa thất bại: " + result.Message;
		}
		return RedirectToAction("Index");
	}

    [HttpGet]
    [Route("restore/{id}")]
    public async Task<IActionResult> Restore(int id)
    {
        var rs = await _promotionService.RestoreSupplierAsync(id);
        if (rs.IsSuccess)
        {
            TempData["success"] = rs.Message;
        }
        else
        {
            TempData["err"] = "Phục hồi nhà cung cấp thất bại: " + rs.Message;
        }
        return RedirectToAction("Index");
    }
}

