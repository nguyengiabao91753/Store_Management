using RetailShop.Blazor.Models;

namespace RetailShop.Blazor.Services.IServices;

public interface ICartService
{
    public List<Cart> GetCart();

    public bool AddToCart(Cart cart);

    public bool ClearCart(List<int> products);

    public bool UpdateCart(Cart cart);
}
