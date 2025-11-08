using System.Threading.Tasks;
using RetailShop.Models;

    public interface IDashboardService
    {
       List<MonthlyRevenueDto> GetMonthlyRevenue();
    List<TopProductDto> GetTopSellingProducts();

    List<Order> GetRecentOrders(DateTime? startDate = null, DateTime? endDate = null, int? month = null, int? year = null, int pageIndex = 1, int pageSize = 10);
    }