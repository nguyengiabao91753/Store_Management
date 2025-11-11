using Humanizer;
using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _appDbContext;

        public ReportService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        //List chi tiet san pham ban trong 1 ngay
        public async Task<List<ReportDetail>> GetReportDetails(DateTime date)
        {
            try
            {
                return await _appDbContext.OrderItems
                    .Where(i=> i.Order.OrderDate.Value.Date == date.Date)
                    .Select(i => new ReportDetail
                    {
                        OrderId = i.OrderId,
                        OrderItemId = i.OrderItemId,
                        ProductName = i.Product.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.Price,
                        Date = i.Order.OrderDate.Value
                    })
                    .OrderByDescending(i => i.Quantity)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi khi lấy dữ liệu. Lỗi: " + e);
                return null;
            }
        }

        //List tong hop doanh thu
        public async Task<List<Report>> GetReports(DateTime from_date, DateTime to_date, string groupBy = "day")
        {
            try
            {
                // đảm bảo OrderDate có giá trị
                var query = from oi in _appDbContext.OrderItems
                            join o in _appDbContext.Orders on oi.OrderId equals o.OrderId
                            where o.OrderDate.HasValue && o.OrderDate.Value >= from_date && o.OrderDate.Value <= to_date
                            select new { oi, o };

                List<Report> result = new List<Report>();

                if (groupBy?.ToLower() == "year")
                {
                    var tmp = await query
                        .GroupBy(x => x.o.OrderDate.Value.Year)
                        .Select(g => new
                        {
                            Year = g.Key,
                            TotalOrders = g.Select(x => x.o.OrderId).Distinct().Count(),
                            TotalProductsSold = g.Sum(x => x.oi.Quantity),
                            TotalRevenue = g.Sum(x => x.oi.Quantity * x.oi.Price)
                        })
                        .ToListAsync();

                    result = tmp
                        .Select(t => new Report
                        {
                            date = new DateTime(t.Year, 1, 1),
                            TotalOrders = t.TotalOrders,
                            TotalProductsSold = t.TotalProductsSold,
                            TotalRevenue = t.TotalRevenue
                        })
                        .OrderBy(r => r.date)
                        .ToList();
                }
                else if (groupBy?.ToLower() == "month")
                {
                    var tmp = await query
                        .GroupBy(x => new { Year = x.o.OrderDate.Value.Year, Month = x.o.OrderDate.Value.Month })
                        .Select(g => new
                        {
                            g.Key.Year,
                            g.Key.Month,
                            TotalOrders = g.Select(x => x.o.OrderId).Distinct().Count(),
                            TotalProductsSold = g.Sum(x => x.oi.Quantity),
                            TotalRevenue = g.Sum(x => x.oi.Quantity * x.oi.Price)
                        })
                        .ToListAsync();

                    result = tmp
                        .Select(t => new Report
                        {
                            date = new DateTime(t.Year, t.Month, 1),
                            TotalOrders = t.TotalOrders,
                            TotalProductsSold = t.TotalProductsSold,
                            TotalRevenue = t.TotalRevenue
                        })
                        .OrderBy(r => r.date)
                        .ToList();
                }
                else // day
                {
                    var tmp = await query
                        .GroupBy(x => new
                        {
                            Year = x.o.OrderDate.Value.Year,
                            Month = x.o.OrderDate.Value.Month,
                            Day = x.o.OrderDate.Value.Day
                        })
                        .Select(g => new
                        {
                            g.Key.Year,
                            g.Key.Month,
                            g.Key.Day,
                            TotalOrders = g.Select(x => x.o.OrderId).Distinct().Count(),
                            TotalProductsSold = g.Sum(x => x.oi.Quantity),
                            TotalRevenue = g.Sum(x => x.oi.Quantity * x.oi.Price)
                        })
                        .ToListAsync();

                    result = tmp
                        .Select(t => new Report
                        {
                            date = new DateTime(t.Year, t.Month, t.Day),
                            TotalOrders = t.TotalOrders,
                            TotalProductsSold = t.TotalProductsSold,
                            TotalRevenue = t.TotalRevenue
                        })
                        .OrderBy(r => r.date)
                        .ToList();
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi khi lấy dữ liệu. Lỗi: " + e);
                return new List<Report>();
            }
        }

        public async Task<List<Report>> GetBestSellingPorducts (DateTime from_date, DateTime to_date)
        {
            try
            {
                var result = await _appDbContext.OrderItems
                                    .Where(oi => oi.Order.OrderDate >= from_date && oi.Order.OrderDate <= to_date)
                                    .GroupBy(oi => oi.ProductId)
                                    .Select(g => new
                                    {
                                        ProductId = g.Key,
                                        TotalSold = g.Sum(x => x.Quantity),
                                        TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                                    })
                                    .OrderByDescending(x => x.TotalSold)
                                    .ToListAsync();

            // chỉ lấy những sản phẩm có bán được ít nhất 1 sản phẩm
            var productIds = result.Where(x => x.TotalSold > 0).Select(x => x.ProductId).ToList();

            var products = await _appDbContext.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId);

            return result
                .Where(x => products.ContainsKey(x.ProductId))
                .Select(x => new Report
                {
                    BestSellingProduct = products[x.ProductId],
                    TotalProductsSold = x.TotalSold,
                    TotalRevenue = x.TotalRevenue
                })
                .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi khi lấy dữ liệu. Lỗi: " + e);
                return new List<Report>();
            }
        }

        public async Task<List<Report>> GetLoyalCustomers(DateTime from_date, DateTime to_date)
        {
            try
            {
                var result = await _appDbContext.Orders
                                     .Where(o => o.OrderDate >= from_date && o.OrderDate <= to_date)
                                     .SelectMany(o => o.OrderItems.Select(i => new
                                     {
                                         o.CustomerId,
                                         o.OrderId,
                                         Revenue = i.Quantity * i.Price
                                     }))
                                     .GroupBy(x => x.CustomerId)
                                     .Select(g => new
                                     {
                                         CustomerId = g.Key,
                                         TotalOrders = g.Select(x => x.OrderId).Distinct().Count(),
                                         TotalRevenue = g.Sum(x => x.Revenue)
                                     })
                                     .OrderByDescending(x => x.TotalOrders)
                                     .ToListAsync();

                // chỉ lấy khách có ít nhất 1 đơn
                var customerIds = result.Where(x => x.TotalOrders > 0).Select(x => x.CustomerId).ToList();

                var customers = await _appDbContext.Customers
                    .Where(c => customerIds.Contains(c.CustomerId))
                    .ToDictionaryAsync(c => c.CustomerId);

                return result
                    .Where(x => customers.ContainsKey(x.CustomerId.Value))
                    .Select(x => new Report
                    {
                        LoyalCustomer = customers[x.CustomerId.Value],
                        TotalOrders = x.TotalOrders,
                        TotalRevenue = x.TotalRevenue
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi khi lấy dữ liệu. Lỗi: " + e);
                return new List<Report>();
            }
        }
    }
}
