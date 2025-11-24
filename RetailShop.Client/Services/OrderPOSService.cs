using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Dtos;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Services;

public class OrderPOSService : IOrderPOSService
{
    private readonly AppDbContext _db;
    public OrderPOSService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Order> PlaceOrderAsync(OrderPlaceDto orderPlaceDto, int customerId)
    {
        using (var transaction = await _db.Database.BeginTransactionAsync())
        {
            try
            {
                

                Order order = new Order()
                {
                    PromoId = orderPlaceDto.PromoId,
                    OrderDate = DateTime.Now,
                    Status = "paid",
                    TotalAmount = orderPlaceDto.TotalAmount
                };

                if (customerId != 0)
                {
                    order.CustomerId = customerId;
                }

                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                bool orderItemsCreated = await CreateOrderItems(orderPlaceDto, order.OrderId);
                if (!orderItemsCreated)
                {
                    transaction.Rollback();
                    return new Order();
                }
                await transaction.CommitAsync();
                return order;

            }
            catch (Exception)
            {
                transaction.Rollback();
                return new Order();
            }
        }
    }

    public async Task<bool> CreateOrderItems(OrderPlaceDto orderPlaceDto, int orderId)
    {
        try
        {
            foreach (var item in orderPlaceDto.Products)
            {
                OrderItem orderItem = new OrderItem()
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Subtotal = item.Price * item.Quantity
                };
                await _db.OrderItems.AddAsync(orderItem);
               
            }
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }
    public async Task<IReadOnlyList<Order>> GetAllOrdersAsync()
    {
        return await _db.Orders
                        .AsNoTracking()
                        .OrderBy(c => c.OrderDate)
                        .ToListAsync();
    }

    public async Task<IReadOnlyList<OrderHistoryRowDto>> GetOrderHistoryRowsAsync()
    {
        var orders = await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Payment)
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var rows = orders.Select(o =>
        {
            var statusRaw = o.Status?.Trim();
            string orderStatus =
                string.Equals(statusRaw, "canceled", StringComparison.OrdinalIgnoreCase) ? "Canceled" :
                (string.Equals(statusRaw, "paid", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(statusRaw, "done", StringComparison.OrdinalIgnoreCase)) ? "Done" :
                "Pending";

            return new OrderHistoryRowDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerName = o.Customer?.Name ?? "Walk-in",
                OrderStatus = orderStatus,
                TotalPayment = o.TotalAmount ?? 0m,
                PaymentStatus = o.Payment != null ? "Paid" : "Unpaid"
            };
        }).ToList();

        return rows;
    }

	public async Task<IReadOnlyList<OrderHistoryRowDto>> GetOrderHistoryRowsAsync(DateTime? start, DateTime? end)
	{
		var query = _db.Orders
			.Include(o => o.Customer)
			.Include(o => o.Payment)
			.AsNoTracking()
			.AsQueryable();

		if (start.HasValue)
		{
			query = query.Where(o => o.OrderDate >= start.Value);
		}
		if (end.HasValue)
		{
			query = query.Where(o => o.OrderDate <= end.Value);
		}

		var orders = await query
			.OrderByDescending(o => o.OrderDate)
			.ToListAsync();

		var rows = orders.Select(o =>
		{
			var statusRaw = o.Status?.Trim();
			string orderStatus =
				string.Equals(statusRaw, "canceled", StringComparison.OrdinalIgnoreCase) ? "Canceled" :
				(string.Equals(statusRaw, "paid", StringComparison.OrdinalIgnoreCase) ||
				 string.Equals(statusRaw, "done", StringComparison.OrdinalIgnoreCase)) ? "Done" :
				"Pending";

			return new OrderHistoryRowDto
			{
				OrderId = o.OrderId,
				OrderDate = o.OrderDate,
				CustomerName = o.Customer?.Name ?? "Not defined Customer",
				OrderStatus = orderStatus,
				TotalPayment = o.TotalAmount ?? 0m,
				PaymentStatus = o.Payment != null ? "Paid" : "Unpaid"
			};
		}).ToList();

		return rows;
	}

	public async Task<Order?> GetOrderByIdWithDetailsAsync(int orderId)
	{
		var order = await _db.Orders
			.Include(o => o.Customer)
			.Include(o => o.User)
			.Include(o => o.Promo)
			.Include(o => o.Payment)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.AsNoTracking()
			.FirstOrDefaultAsync(o => o.OrderId == orderId);

		return order;
	}
}
