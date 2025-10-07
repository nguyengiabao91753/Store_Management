using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RetailShop.Models;

[Index("PromoCode", Name = "UQ__Promotio__32DBED35D65AA90B", IsUnique = true)]
public partial class Promotion
{
    [Key]
    public int PromoId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PromoCode { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? DiscountType { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal DiscountValue { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MinOrderAmount { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Status { get; set; }

    [InverseProperty("Promo")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
