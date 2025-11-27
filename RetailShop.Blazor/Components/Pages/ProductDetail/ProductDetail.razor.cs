using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RetailShop.Blazor.Components.Shared;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Components.Pages.ProductDetail;

public partial class ProductDetail
{
    [Parameter]
    public int ProductId { get; set; }

    [Inject]
    private IProductService ProductService { get; set; }

    [Inject]
    private ICartService CartService { get; set; }
    private ProductDTO? product;
    private List<Product> relatedProducts = new();
    private List<string> productImages = new();
    private int quantity = 1;
    private bool isWishlisted = false;

    protected override async Task OnInitializedAsync()
    {
        // Simulate loading product data
        await Task.Delay(500);

        var response = await ProductService.GetProductById(ProductId);
        product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));

        // Load related products
        relatedProducts = GetRelatedProducts();
    }

    private void IncreaseQuantity()
    {
        if (quantity < 99) quantity++;
    }

    private void DecreaseQuantity()
    {
        if (quantity > 1) quantity--;
    }

    private void AddToCart()
    {
        // Add to cart logic here
        Console.WriteLine($"Added {quantity} x {product.ProductName} to cart");
        var rs = CartService.AddToCart(new Models.Cart
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Price = product.Price,
            Quantity = quantity,
            TotalAmount = product.Price * quantity,
            ProductImage = product.ProductImage
        });
        if (rs)
        {
            Console.WriteLine("Product added to cart successfully.");
            ToastRef?.ShowToast(
           "Added to Cart!",
           $"{quantity}x {product.ProductName}",
           Toast.ToastType.Success
       );
        }
        else
        {
            Console.WriteLine("Failed to add product to cart.");
        }

    }

    private void ToggleWishlist()
    {
        isWishlisted = !isWishlisted;
    }

    private List<Product> GetRelatedProducts()
    {
        // Simulate related products
        return new List<Product>
        {
            new Product { ProductId = 2, ProductName = "Smart Watch", CategoryName = "Electronics", Price = 199.99m, ProductImage = "/images/watch.jpg" },
            new Product { ProductId = 3, ProductName = "Laptop Stand", CategoryName = "Accessories", Price = 49.99m, ProductImage = "/images/stand.jpg" },
            new Product { ProductId = 4, ProductName = "USB-C Cable", CategoryName = "Accessories", Price = 19.99m, ProductImage = "/images/cable.jpg" },
            new Product { ProductId = 5, ProductName = "Phone Case", CategoryName = "Accessories", Price = 29.99m, ProductImage = "/images/case.jpg" }
        };
    }

    // Product model (add to your Models folder)
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string ProductImage { get; set; }
    }
}
