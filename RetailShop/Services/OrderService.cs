using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Services;
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        public OrderService(AppDbContext db)
        {
            _db = db;
        }
    public async Task<ResultService<Order>> CreateOrderAsync(Order order)
    {
        var rs = new ResultService<Order>();
        try
        {
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = order;
            rs.Message = "Order created successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error creating order: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<List<Order>>> GetAllOrdersAsync()
    {
        var rs = new ResultService<List<Order>>();
        try
        {
            var orders = await _db.Orders.ToListAsync();
            rs.IsSuccess = true;
            rs.Message = "Order retrieved successfully";
            rs.Data = orders;
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving orders: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Order>> GetOrderByIdAsync(int id)
    {
        var rs = new ResultService<Order>();
        try
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Order not found.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = order;
                rs.Message = "Order retrieved successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving order: {ex.Message}";
        }
        return rs;
    }

    public async Task<ResultService<Order>> UpdateOrderAsync(Order order)
    {
        var rs = new ResultService<Order>();
        try
        {
            var existingOrder = await _db.Orders.FindAsync(order.OrderId);
            if (existingOrder == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Order not found.";
                return rs;
            }
            _db.Entry(existingOrder).CurrentValues.SetValues(order);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = existingOrder;
            rs.Message = "Order updated successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error updating order: {ex.Message}";
        }
        return rs;
    }

}

