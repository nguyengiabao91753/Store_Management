using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Services.IServices;

public interface IPromotionService
{
    Task<ResponseDto?> GetPromotionByCode(string code);

}
