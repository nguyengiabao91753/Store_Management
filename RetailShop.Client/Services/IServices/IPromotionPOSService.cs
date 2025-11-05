using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices;

public interface IPromotionPOSService
{
    Task<List<Promotion>> GetAllActivePromotionsAsync();

   Promotion? GetPromotionByCode(string code);

    Task<Promotion?> UpdateUseCount(int promotionId);
}
