using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Services
{
    public class InventoryReportService : IInventoryReportService
    {
        private readonly AppDbContext _context;

        public InventoryReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryStatisticModel>> GetInventoryReport(DateTime? fromDate, DateTime? toDate)
        {
            var products = await (from p in _context.Products
                                  join inv in _context.Inventories on p.ProductId equals inv.ProductId
                                  select new
                                  {
                                      p.ProductId,
                                      p.ProductName,
                                      p.ProductImage,
                                      p.Price,
                                      Stock = inv.Quantity ?? 0
                                  }).ToListAsync();

            var list = new List<InventoryStatisticModel>();

            foreach (var item in products)
            {
                var sold = await _context.OrderItems
                    .Where(o => o.ProductId == item.ProductId &&
                                (!fromDate.HasValue || o.Order.OrderDate >= fromDate) &&
                                (!toDate.HasValue || o.Order.OrderDate <= toDate))
                    .SumAsync(o => (int?)o.Quantity) ?? 0;

                int remaining = item.Stock - sold;

                list.Add(new InventoryStatisticModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Image = item.ProductImage,
                    ProductPrice = item.Price,

                    StockQuantity = item.Stock,
                    SoldQuantity = sold,
                    RemainingQuantity = remaining
                });
            }

            return list
                .OrderByDescending(x => x.SoldQuantity)
                .Select((x, idx) =>
                {
                    x.Rank = idx + 1;
                    return x;
                })
                .ToList();
        }
    }
}
