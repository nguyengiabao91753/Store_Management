using RetailShop.Client.Dtos;
using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices;

public interface IOrderPOSService
{
    Task<Order> PlaceOrderAsync(OrderPlaceDto orderPlaceDto, int customerId = 0);
}
