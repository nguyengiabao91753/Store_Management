using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Controllers;
public class PromotionController : Controller
{
    private readonly IPromotionPOSService _promotionPOSService;

    public PromotionController(IPromotionPOSService promotionPOSService)
    {
        _promotionPOSService = promotionPOSService;
    }

    [HttpPost]
    public IActionResult CheckCode(string code)
    {
       var promo = _promotionPOSService.GetPromotionByCode(code);

        if (promo == null)
        {
            return Json(new { isValid = false });
        }

        return Json(new
        {
            isValid = true,
            promoId = promo.PromoId,
            discountPercent = promo.DiscountType == "percent" ? promo.DiscountValue : 0,
            discountAmount = promo.DiscountType == "fixed" ? promo.DiscountValue : 0,
            minValue = promo.MinOrderAmount
        });
    }

}
