using RetailShop.Client.Dtos;
using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices;

public interface IOrderPOSService
{
    Task<Order> PlaceOrderAsync(OrderPlaceDto orderPlaceDto, int customerId = 0);
	Task<IReadOnlyList<Order>> GetAllOrdersAsync();

	// Returns a lightweight projection for Order History Index UI
	Task<IReadOnlyList<OrderHistoryRowDto>> GetOrderHistoryRowsAsync();

	// Returns projection filtered by date range (inclusive). Nulls mean no bound.
	Task<IReadOnlyList<OrderHistoryRowDto>> GetOrderHistoryRowsAsync(DateTime? start, DateTime? end);

	// Returns an Order with all related details for the Details view
	Task<Order?> GetOrderByIdWithDetailsAsync(int orderId);
}
