using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailShop.API.Dtos;

public class PromotionDTO
{
    public int PromoId { get; set; }
    public string PromoCode { get; set; } = null!;

    public string? Description { get; set; }

    public string? DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }
    public string? Status { get; set; }
}
