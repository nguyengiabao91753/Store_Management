using PayPal.Api;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Ubility;

namespace RetailShop.Blazor.Services;

public class PaymentService : IPaymentService
{
    
    private readonly HttpClient _http;

    public PaymentService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> CreatePaypal(OrderPlaceDto orderPlaceDto)
    {
        var apiUrl = SD.ServierAPI + "/api/paypal/create";

        var response = await _http.PostAsJsonAsync(apiUrl, orderPlaceDto);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadFromJsonAsync<PaypalResponse>();
        return json?.Approval_url;

    }

    public async Task<bool> ExecutePaypal(string approve_url)
    {
        var rs = await _http.GetFromJsonAsync<Payment>(approve_url);

        if (rs != null)
            return true;
        return false;
    }

}
