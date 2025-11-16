namespace RetailShop.Client.Models
{
    public class Report
    {
        public decimal? TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }
        public DateTime date { get; set; }

        public Product BestSellingProduct { get; set; }
        public Customer LoyalCustomer { get; set; }
    }
}
