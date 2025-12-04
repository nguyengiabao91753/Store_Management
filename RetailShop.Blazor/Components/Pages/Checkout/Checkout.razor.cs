using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
    [Inject]
    private IPaymentService PaymentService { get; set; } = default!;

    [Inject] IJSRuntime JS { get; set; } = default!;

    private OrderPlaceDto orderDto = new();
    private List<Cart> cartItems = new();
    private decimal subtotal = 0;
    private string promoCode = "";
    private string promoMessage = "";
    private bool promoSuccess = false;
    private bool showValidation = false;
    private bool isPlacingOrder = false;
    private bool isApplyingPromo = false;

    private string selectedEWallet = "";

    private Toast? ToastRef;

    private DotNetObjectReference<Checkout> objRef;


    protected override void OnInitialized()
    {
        objRef = DotNetObjectReference.Create(this);

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
        if (method != "e-wallet")
        {
            selectedEWallet = "";
        }
    }

    private void SelectEWallet(string provider)
    {
        selectedEWallet = provider;
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
            if (promo.DiscountType == "percent")
            {
                
                orderDto.DiscountAmount = subtotal * (promo.DiscountValue / 100);
               
                promoMessage = $"Promo code applied! You saved {promo.DiscountValue}%";
            }
            else if (promo.DiscountType == "fixed")
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


        var rs = new ResponseDto();
        if (orderDto.PaymentMethod =="e-wallet" && string.IsNullOrWhiteSpace(selectedEWallet))
        {
            ToastRef?.ShowToast(
          "Please select an e-wallet provider!",
          null,
          Toast.ToastType.Error
            );
            isPlacingOrder = false;
            return;
        }
        else if(orderDto.PaymentMethod == "e-wallet")
        {
            //Gọi API create Paypal
            var paypalResult = await PaymentService.CreatePaypal(orderDto);
            if (paypalResult == null || string.IsNullOrWhiteSpace(paypalResult))
            {
                ToastRef?.ShowToast("Failed to create PayPal payment!", null, Toast.ToastType.Error);
                isPlacingOrder = false;
                return;
            }
            // Redirect user to PayPal approval URL
            await JS.InvokeVoidAsync("openPaypalPopup", paypalResult, objRef);

            return;
        }
        else
        {
             rs = await OrderService.PlaceOrderAsync(orderDto);

        }

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


    [JSInvokable]
    public  Task OnPaypalSuccess(string orderId)
    {
        // clear cart
        CartService.ClearCart(null);
        Nav.NavigateTo($"/checkout/success?orderId={orderId}");
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnPaypalFailed()
    {
        ToastRef?.ShowToast("Payment failed!", null, Toast.ToastType.Error);
        return Task.CompletedTask;
    }
}
