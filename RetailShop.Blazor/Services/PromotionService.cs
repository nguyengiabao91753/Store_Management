using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Ubility;

namespace RetailShop.Blazor.Services;

public class PromotionService : IPromotionService
{
    private readonly IBaseService _baseService;
    public PromotionService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public async Task<ResponseDto?> GetPromotionByCode(string code)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,

            Url = SD.ServierAPI + $"/api/promotion/get-promotion/{code}"
        });
    }
}
