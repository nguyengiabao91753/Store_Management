
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RetailShop.Blazor.Models;
using RetailShop.Blazor.Services.IServices;
using System.Threading.Tasks;

namespace RetailShop.Blazor.Components.Pages.CartPage;

public partial class CartPage
{
    [Inject]
    public ICartService CartService { get; set; } = default!;
    private List<Cart> Carts { get; set; } = new();

    private decimal Subtotal => Carts.Sum(c => c.TotalAmount!.Value);
    private decimal Shipping => Subtotal > 100 ? 0 : 10;
    private decimal Tax => Subtotal * 0.1m;
    private decimal Total => Subtotal + Shipping + Tax;

    protected override void OnInitialized()
    {
        if(CustomerStateService.IsAuthenticated == false)
        {
           
            Nav.NavigateTo("/login");
            return;
        }

        var rs = CartService.GetCart();
        Carts = rs;

    }

    private void IncreaseQuantity(Cart item)
    {
        item.Quantity++;
        CartService.UpdateCart(item);
    }

    private void DecreaseQuantity(Cart item)
    {
        if (item.Quantity > 1)
        {
            item.Quantity--;
            CartService.UpdateCart(item);
        }
    }

    private void RemoveItem(Cart item)
    {
        Carts.Remove(item);
    }

    private void GoToCheckout()
    {
        Nav.NavigateTo("/checkout");
    }


}
