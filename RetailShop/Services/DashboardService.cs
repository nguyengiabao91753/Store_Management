using Microsoft.EntityFrameworkCore;
using RetailShop.Models;
using RetailShop.Services.IServices;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using RetailShop.Data;


public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext context)
    {
        _db = context;
    }
    // Biểu đồ doanh thu
    public List<MonthlyRevenueDto> GetMonthlyRevenue()
    {
        return _db.Orders
            .Where(o => o.Status == "paid" && o.OrderDate != null)
            .GroupBy(o => new { o.OrderDate!.Value.Year, o.OrderDate!.Value.Month })
            .Select(g => new MonthlyRevenueDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Total = g.Sum(x => (decimal)(x.TotalAmount ?? 0))
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();
    }

    // Top sản phẩm bán chạy
    public List<TopProductDto> GetTopSellingProducts()
    {
        return _db.OrderItems
            .Where(i => i.Order != null && i.Order.OrderDate != null) // tránh null
            .GroupBy(i => new
            {
                i.Product.ProductName,
                Month = i.Order.OrderDate!.Value.Month,
                Year = i.Order.OrderDate!.Value.Year
            })
            .Select(g => new TopProductDto
            {
                ProductName = g.Key.ProductName,
                Month = g.Key.Month,
                Year = g.Key.Year,
                QuantitySold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToList();
    }
public List<Order> GetRecentOrders(DateTime? startDate = null, DateTime? endDate = null, int? month = null, int? year = null, int pageIndex = 1, int pageSize = 10)
{
    var query = _db.Orders
        .Include(o => o.Customer) // load thêm tên khách hàng
        .AsQueryable();

    // Lọc theo khoảng thời gian
    if (startDate.HasValue)
        query = query.Where(o => o.OrderDate >= startDate.Value);

    if (endDate.HasValue)
        query = query.Where(o => o.OrderDate <= endDate.Value);

    // Lọc theo tháng / năm
    if (month.HasValue && month.Value > 0)
        query = query.Where(o => o.OrderDate!.Value.Month == month.Value);

    if (year.HasValue && year.Value > 0)
        query = query.Where(o => o.OrderDate!.Value.Year == year.Value);

    // Phân trang & sắp xếp
    return query
        .OrderByDescending(o => o.OrderDate)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToList();
}



}