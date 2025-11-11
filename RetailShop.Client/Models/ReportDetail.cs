namespace RetailShop.Client.Models
{
    public class ReportDetail
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Revenue => Quantity * UnitPrice; // tính doanh thu từng sản phẩm
        public DateTime Date { get; set; }
    }
}