using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailShop.Blazor.Models;

public class Cart
{
    [Key]
    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
