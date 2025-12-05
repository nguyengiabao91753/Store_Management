using System.ComponentModel.DataAnnotations.Schema;

namespace RetailShop.API.Dtos;

public class OrderPlaceDto
{
    public List<ProductDTO> Products
    {
        get; set;
    }

   public int? PromoId { get; set; }

    public string PaymentMethod { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public int? CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }
}
