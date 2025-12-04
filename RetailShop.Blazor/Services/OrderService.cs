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

    public async Task<ResponseDto?> GetOrderById(int orderId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ServierAPI + "/api/order/get-order-by-id/" + orderId
        });
    }

    public async Task<ResponseDto?> GetOrdersByCustomer(int CusId)
    {
       return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ServierAPI + "/api/order/get-orders-by-customer/" + CusId
        });
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
