using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Services;
using RetailShop.API.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

//Use mapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<DataMapping>());

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

// Add services to the container.
builder.Services.AddScoped<IProductAPIService, ProductAPIService>();
builder.Services.AddScoped<IOrderAPIService, OrderAPIService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryAPIService, CategoryAPIService>();
builder.Services.AddScoped<ISupplierAPIService, SupplierAPIService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
