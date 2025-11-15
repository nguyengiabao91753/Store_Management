using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Controllers;

    public class OrderHistoryController : Controller
    {
        private readonly IOrderPOSService _orderPOSService;
        private readonly AppDbContext _db;

        public OrderHistoryController(IOrderPOSService orderPOSService, AppDbContext db)
        {
            _orderPOSService = orderPOSService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.orders = await _orderPOSService.GetAllOrdersAsync();
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.Promo)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
