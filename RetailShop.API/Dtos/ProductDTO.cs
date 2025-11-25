using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailShop.API.Dtos;

public class ProductDTO
{
    public int ProductId { get; set; }


    //Category
    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; } = null!;
    /////////////////////////
   
    //Supplier
    public int SupplierId { get; set; }
    public string? SupplierName { get; set; } = null!;
    public string? SupplierPhone { get; set; }
    public string? SupplierEmail { get; set; }
    public string? SupplierAddress { get; set; }
    //////////////////////////

    //Product
    public string ProductName { get; set; } = null!;

    public string? ProductImage { get; set; }


    public string? Barcode { get; set; }

    public decimal Price { get; set; }

    public string? Unit { get; set; }

    public DateTime? CreatedAt { get; set; }


    public bool Active { get; set; } = true;

    public int Quantity { get; set; }

}
