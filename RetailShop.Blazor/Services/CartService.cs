using RetailShop.Blazor.Data;
using RetailShop.Blazor.Models;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _db;

    public CartService(AppDbContext db)
    {
        _db = db;
    }

    public bool AddToCart(Cart cart)
    {
        try
        {
            var existingCartItem = _db.Carts.FirstOrDefault(c => c.ProductId == cart.ProductId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cart.Quantity;
                existingCartItem.TotalAmount = existingCartItem.Quantity * existingCartItem.Price;
                _db.Carts.Update(existingCartItem);
            }
            else
            {
                cart.CreatedAt = DateTime.Now;
                cart.TotalAmount = cart.Price * cart.Quantity;
                _db.Carts.Add(cart);
            }
            _db.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool ClearCart(List<int> products)
    {
        try
        {
            var cartItems = _db.Carts.Where(c => products.Contains(c.ProductId)).ToList();
            _db.Carts.RemoveRange(cartItems);
            _db.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateCart(Cart cart)
    {
        var cartex = _db.Carts.FirstOrDefault(c => c.ProductId == cart.ProductId);
        cartex.Quantity = cart.Quantity;
        cartex.Price = cart.Price;
        cartex.TotalAmount = cart.Quantity * cart.Price;
        _db.Carts.Update(cartex);
        return _db.SaveChanges() >0;
    }

    public List<Cart> GetCart()
    {
        var cartItems = _db.Carts.ToList();
        return cartItems;
    }
}
