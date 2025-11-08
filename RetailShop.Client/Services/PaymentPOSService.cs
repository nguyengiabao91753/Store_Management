using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Services;

public class PaymentPOSService : IPaymentPOSService
{
    private readonly AppDbContext _db;
    public PaymentPOSService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<Payment> ProcessPaymentAsync(Payment payment)
    {
        try
        {
            payment.PaymentDate = DateTime.Now;
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }
        catch (Exception ex)
        {
            return new Payment();

        }
    }
}
