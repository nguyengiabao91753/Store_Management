using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices
{
    public interface IOrderItemService
    {
        Task<ResultService<List<OrderItem>>> GetAllOrderItemsAsync(int orderID);
        Task<ResultService<OrderItem>> GetOrderItemByIdAsync(int id);
        Task<ResultService<OrderItem>> CreateOrderItemAsync(OrderItem OrderItem);
    }
}
