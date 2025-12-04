using Azure;
using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices;

public interface IOrderAPIService
{
    Task<ResponseDto?> PlaceOrderAsync(OrderPlaceDto orderPlaceDto, int customerId = 0);

    Task<ResponseDto?> CancleOrderAsync(int orderId);

    Task<ResponseDto?> GetOrdersByCustomer(int CusId);

    Task<ResponseDto?> GetOrderById (int orderId);
}
