using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Models;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _db;
    public PaymentService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<ResponseDto> CreatePayment(Payment payment)
    {
        var response = new ResponseDto();
        try
        {
            var order = await _db.Orders.FindAsync(payment.OrderId);
            if (order == null)
            {
                response.IsSuccess = false;
                response.Message = "Order not found";
                return response;
            }
            payment.Amount = order.TotalAmount ?? 0;
            await _db.Payments.AddAsync(payment);
            await _db.SaveChangesAsync();
            response.IsSuccess = true;

        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;

        }
        return response;
    }
}
