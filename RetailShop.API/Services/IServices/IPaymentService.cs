using RetailShop.API.Dtos;
using RetailShop.API.Models;

namespace RetailShop.API.Services.IServices;

public interface IPaymentService
{
    Task<ResponseDto> CreatePayment (Payment payment);
}
