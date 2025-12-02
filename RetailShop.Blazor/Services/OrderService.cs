using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Ubility;

namespace RetailShop.Blazor.Services;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService;
    public OrderService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public async Task<ResponseDto?> PlaceOrderAsync(OrderPlaceDto orderPlace)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = orderPlace,
            Url = SD.ServierAPI + "/api/order/order-place"
        });
    }
}
