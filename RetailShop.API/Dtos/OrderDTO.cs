using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailShop.API.Dtos;

public class OrderDTO
{
    public int? OrderId { get; set; } = null;

    //Customer
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    /////////////////////////////////////////////
    //User
    public int? UserId { get; set; }
    public string Username { get; set; } = null!;
    ////////////////////////////////////////////
    //Promotion
    public string? PromoCode { get; set; }
    ////////////////////////////////////////////
    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public List<OrderItemDTO>? OrderItems { get; set; }
}
