using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RetailShop.Client.Models;

[Index("Barcode", Name = "UQ__Products__177800D309195C4D", IsUnique = true)]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    public int? SupplierId { get; set; }

    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [Unicode(false)]
    public string? ProductImage { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Barcode { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Unit { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }


    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [NotMapped]
    public bool Active { get; set; } = true;

    [InverseProperty("Product")]
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    [NotMapped]
    public Inventory? Inventory
    {
        get => Inventories.FirstOrDefault();
        set
        {
            if (value != null)
            {
                Inventories.Clear();
                Inventories.Add(value);
            }
        }
    }

    [NotMapped]
    public int Quantity { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("SupplierId")]
    [InverseProperty("Products")]
    public virtual Supplier? Supplier { get; set; }

}
