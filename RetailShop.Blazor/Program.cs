using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using RetailShop.Blazor.Components;
using RetailShop.Blazor.Data;
using RetailShop.Blazor.Services;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Ubility;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=store-client.db"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.Configure<CircuitOptions>(options => options.DetailedErrors = true);



SD.ServierAPI = builder.Configuration.GetValue<string>("Urls:Server");

//Add Services
builder.Services.AddHttpClient();

//Dependency Injection for Services
builder.Services.AddScoped<IBaseService, BaseService>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICustomerAuthService, CustomerAuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddSingleton<CustomerStateService>();


var app = builder.Build();

//Apply Migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
