namespace RetailShop.Client.Dtos;

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductImage { get; set; }
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public string? Unit { get; set; }

    public int Quantity { get; set; }
}
