using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices;
public interface IOrderService
{
    Task<ResultService<List<Order>>> GetAllOrdersAsync();
    Task<ResultService<Order>> GetOrderByIdAsync(int id);
    Task<ResultService<Order>> CreateOrderAsync(Order Order);
    Task<ResultService<Order>> UpdateOrderAsync(Order Order);
}