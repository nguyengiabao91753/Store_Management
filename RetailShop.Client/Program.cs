using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Extension;
using RetailShop.Client.Services;
using RetailShop.Client.Services.IServices;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/login/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Cookie sẽ hết hạn sau 8 giờ, sau đó redirect đến /login
        options.SlidingExpiration = true; // Tự động gia hạn khi user hoạt động (gửi request)
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
// Đọc connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


SD.AdminUrl = builder.Configuration["Endpoints:Admin"];

// Thêm DbContext vào DI container  
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        //sqlOptions => sqlOptions.EnableRetryOnFailure() // optional resilience
    ));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPromotionPOSService, PromotionPOSService>();
builder.Services.AddScoped<ICustomerPOSService, CustomerPOSService>();
builder.Services.AddScoped<IPaymentPOSService, PaymentPOSService>();
builder.Services.AddScoped<IInventoryPOSService, InventoryPOSService>();
builder.Services.AddScoped<IOrderPOSService, OrderPOSService>();
builder.Services.AddScoped<IUserPOSService, UserPOSService>();



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
