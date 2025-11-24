using System.Collections.Generic;

namespace RetailShop.API.Models;
public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Total { get; set; }
    }

    public class TopProductDto
    {
        public string ProductName { get; set; } = "";
    public int QuantitySold { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }

    }

    public class DashboardViewModel
    {
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }