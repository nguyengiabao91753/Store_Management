using System.ComponentModel.DataAnnotations.Schema;

namespace RetailShop.Blazor.Dtos;

public class OrderItemDTO
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    //Product
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;

    public string? ProductImage { get; set; }
    public string? Barcode { get; set; }
    public string? Unit { get; set; }
    //////////////////////////////////////////////////

    public int Quantity { get; set; }

    
    public decimal Price { get; set; }

   
    public decimal? Subtotal { get; set; }
}
