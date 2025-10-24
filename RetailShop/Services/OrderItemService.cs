using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Services;

    public class OrderItemService : IOrderItemService
    {
        private readonly AppDbContext _db;
        public OrderItemService(AppDbContext db)
        {
            _db = db;
        }

    public async Task<ResultService<OrderItem>> CreateOrderItemAsync(OrderItem OrderItem)
    {
        var rs = new ResultService<OrderItem>();
        try
        {
            await _db.OrderItems.AddAsync(OrderItem);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = OrderItem;
            rs.Message = "OrderItem created successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error creating OrderItem: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<List<OrderItem>>> GetAllOrderItemsAsync(int orderID)
    {
        var rs = new ResultService<List<OrderItem>>();
        try
        {
            var orderItems = await _db.OrderItems
                .Where(oi => oi.OrderId == orderID)
                .ToListAsync();
            rs.IsSuccess = true;
            rs.Message = "OrderItems retrieved successfully";
            rs.Data = orderItems;
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving OrderItems: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<OrderItem>> GetOrderItemByIdAsync(int id)
    {
        var rs = new ResultService<OrderItem>();
        try
        {
            var orderItem = await _db.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                rs.IsSuccess = false;
                rs.Message = "OrderItem not found.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = orderItem;
                rs.Message = "OrderItem retrieved successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving OrderItem: {ex.Message}";
        }
        return rs;
    }
}

