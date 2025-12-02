using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices;

public interface IPromotionService
{
    Task<ResponseDto> UpdatePromotionCount(int promoId);

    Task<ResponseDto> GetPromotionByCode(string code);
}
