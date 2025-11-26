using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices;

public interface IOrderAPIService
{
    Task<ResponseDto?> PlaceOrderAsync(OrderPlaceDto orderPlaceDto, int customerId = 0);

}
