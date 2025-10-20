using RetailShop.Data;
using RetailShop.Services.IServices;

namespace RetailShop.Services;

public class Example : IExample
{
    private readonly AppDbContext _db;
    public Example(AppDbContext db)
    {
        _db = db;
    }
    public string GetMessage()
    {
        var us = _db.Users.FirstOrDefault();
        return "Hello from Example service: " + (us?.Username ?? "Guest");
    }
}
