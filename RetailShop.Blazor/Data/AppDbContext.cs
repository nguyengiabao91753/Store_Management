using Microsoft.EntityFrameworkCore;
using RetailShop.Blazor.Models;

namespace RetailShop.Blazor.Data;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Cart> Carts { get; set; }
}
