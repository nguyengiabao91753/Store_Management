using Microsoft.AspNetCore.Authentication.Cookies; // <-- Cần cho Cookie Auth
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Models;
using RetailShop.Services;
using RetailShop.Services.IServices;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Chỉ định tên Scheme, bạn có thể dùng mặc định
        options.LoginPath = "/login"; // <-- Đặt trang đăng nhập (Action Index của LoginController)
        options.LogoutPath = "/login/logout"; // <-- Đặt trang đăng xuất (Action Logout của LoginController
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Thời gian hết hạn của cookie (ví dụ: 30 phút)
        options.SlidingExpiration = true; 
        options.Cookie.HttpOnly = true; 
        options.Cookie.IsEssential = true;
    });


// Đọc connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Thêm DbContext vào DI container  
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure() // optional resilience
    ));
    




#region Add De[pendency Injection for Services
builder.Services.AddScoped<IExample, Example>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<IUserService, UserService> ();
builder.Services.AddScoped<IProductService, ProductService>();

#endregion

// Cloudinary Configuration
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
