using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Services.IServices;

public interface IPaymentService
{
    Task<string> CreatePaypal(OrderPlaceDto orderPlaceDto);
    Task<bool> ExecutePaypal(string approve_url);
}
