using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RetailShop.Blazor.Components.Shared;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Models;
using RetailShop.Blazor.Services;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Components.Pages.Checkout;

public partial class Checkout
{
    [Inject]
    private ICartService CartService { get; set; } = default!;
    [Inject]
    private IOrderService OrderService { get; set; } = default!;
    [Inject]
    private IPromotionService PromotionService { get; set; } = default!;
    private OrderPlaceDto orderDto = new();
    private List<Cart> cartItems = new();
    private decimal subtotal = 0;
    private string promoCode = "";
    private string promoMessage = "";
    private bool promoSuccess = false;
    private bool showValidation = false;
    private bool isPlacingOrder = false;
    private bool isApplyingPromo = false;

    private Toast? ToastRef;

    protected override void OnInitialized()
    {
        LoadCartItems();
        orderDto.PaymentMethod = "Cash"; 
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
        await Task.Delay(500); 
        var rs = await PromotionService.GetPromotionByCode(promoCode);
        var promo = JsonConvert.DeserializeObject<PromotionDTO>(Convert.ToString(rs.Result));

        if (promo != null) {
            if (promo.MinOrderAmount != null && subtotal < promo.MinOrderAmount)
            {
                promoMessage = $"Minimum order amount for this promo is {promo.MinOrderAmount:C}";
                promoSuccess = false;
                orderDto.PromoId = null;
                orderDto.DiscountAmount = 0;
                CalculateTotal();
                isApplyingPromo = false;
                return;
            }
            orderDto.PromoId = promo.PromoId;
            if (promo.DiscountType == "Percentage")
            {
                orderDto.DiscountAmount = subtotal * (promo.DiscountValue / 100);
                promoMessage = $"Promo code applied! You saved {promo.DiscountValue}%";
            }
            else if (promo.DiscountType == "Fixed")
            {
                orderDto.DiscountAmount = promo.DiscountValue;
                promoMessage = $"Promo code applied! You saved {promo.DiscountValue:C}";
            }
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

        var rs = await OrderService.PlaceOrderAsync(orderDto);
        if (rs.IsSuccess)
        {
            // Order placed successfully
            // Clear cart and navigate to success page
            CartService.ClearCart(null);
            Nav.NavigateTo("/checkout/success");
            return;
        }
        else
        {
            ToastRef?.ShowToast(
          "Checkout Failed! Please Try Again!",
          null,
          Toast.ToastType.Error
            );


        }

            isPlacingOrder = false;

      
    }
}
