using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices;

public interface IPaymentPOSService
{
    Task<Payment> ProcessPaymentAsync(Payment payment);
}
