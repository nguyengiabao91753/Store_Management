using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices;

public interface IOrderAPIService
{
    Task<OrderDTO> PlaceOrderAsync(OrderPlaceDto orderPlaceDto, int customerId = 0);

}
