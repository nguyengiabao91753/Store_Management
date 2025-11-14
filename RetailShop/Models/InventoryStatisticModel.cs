namespace RetailShop.Models
{
    public class InventoryStatisticModel
    {
        public int Rank { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? Image { get; set; }

        public int StockQuantity { get; set; }        
        public int SoldQuantity { get; set; }         
        public int RemainingQuantity { get; set; }    

        public bool IsLowStock => RemainingQuantity <= 10;
        public decimal ProductPrice { get; set; }
    }
}
