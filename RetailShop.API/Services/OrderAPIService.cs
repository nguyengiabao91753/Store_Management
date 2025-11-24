using AutoMapper;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Models;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Services;

public class OrderAPIService : IOrderAPIService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public OrderAPIService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OrderDTO> PlaceOrderAsync(OrderPlaceDto orderPlaceDto, int customerId = 0)
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
                    return new OrderDTO();
                }
                await transaction.CommitAsync();
                return _mapper.Map<OrderDTO>(order);

            }
            catch (Exception)
            {
                transaction.Rollback();
                return new OrderDTO();
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
}
