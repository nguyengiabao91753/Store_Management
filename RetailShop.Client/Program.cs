using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Extension;
using RetailShop.Client.Services;
using RetailShop.Client.Services.IServices;
using RetailShop.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
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
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IInventoryReportService, InventoryReportService>();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
