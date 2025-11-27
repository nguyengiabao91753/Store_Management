using Microsoft.AspNetCore.Components;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Models;
using RetailShop.Blazor.Services;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Components.Pages.Checkout;

public partial class Checkout
{
    [Inject]
    private ICartService CartService { get; set; } = default!;
    private OrderPlaceDto orderDto = new();
    private List<Cart> cartItems = new();
    private decimal subtotal = 0;
    private string promoCode = "";
    private string promoMessage = "";
    private bool promoSuccess = false;
    private bool showValidation = false;
    private bool isPlacingOrder = false;
    private bool isApplyingPromo = false;

    protected override void OnInitialized()
    {
        LoadCartItems();
        orderDto.PaymentMethod = "Cash"; // Default payment method
    }

    private void LoadCartItems()
    {
        cartItems = CartService.GetCart();
        CalculateTotal();
    }

    private void CalculateTotal()
    {
        subtotal = cartItems.Sum(x => x.Price * x.Quantity);
        orderDto.TotalAmount = subtotal - (orderDto.DiscountAmount ?? 0);

        // Convert cart items to ProductDTO
        orderDto.Products = cartItems.Select(x => new ProductDTO
        {
            ProductId = x.ProductId,
            ProductName = x.ProductName,
            Price = x.Price,
            Quantity = x.Quantity
        }).ToList();
    }

    private void SelectPaymentMethod(string method)
    {
        orderDto.PaymentMethod = method;
    }

    private async Task ApplyPromo()
    {
        if (string.IsNullOrWhiteSpace(promoCode))
        {
            promoMessage = "Please enter a promo code";
            promoSuccess = false;
            return;
        }

        isApplyingPromo = true;
        await Task.Delay(500); // Simulate API call

        // Mock promo validation - Replace with actual API call
        if (promoCode.ToUpper() == "SAVE10")
        {
            orderDto.PromoId = 1;
            orderDto.DiscountAmount = subtotal * 0.10m; // 10% discount
            promoMessage = "Promo code applied! You saved 10%";
            promoSuccess = true;
        }
        else if (promoCode.ToUpper() == "SAVE20")
        {
            orderDto.PromoId = 2;
            orderDto.DiscountAmount = subtotal * 0.20m; // 20% discount
            promoMessage = "Promo code applied! You saved 20%";
            promoSuccess = true;
        }
        else
        {
            promoMessage = "Invalid promo code";
            promoSuccess = false;
            orderDto.PromoId = null;
            orderDto.DiscountAmount = 0;
        }

        CalculateTotal();
        isApplyingPromo = false;
    }

    private async Task PlaceOrder()
    {
        showValidation = true;

        // Validation
        if (string.IsNullOrWhiteSpace(orderDto.CustomerName) ||
            string.IsNullOrWhiteSpace(orderDto.CustomerPhone) ||
            string.IsNullOrWhiteSpace(orderDto.PaymentMethod))
        {
            return;
        }

        if (!cartItems.Any())
        {
            // Show error message
            return;
        }

        isPlacingOrder = true;

        // Simulate API call - Replace with actual API
        await Task.Delay(1500);

        // Clear cart and navigate to success page
        // CartService.ClearCart();
        // Nav.NavigateTo("/order-success");

        isPlacingOrder = false;

        // For demo, just show alert
        // In production, navigate to order confirmation page
    }
}
