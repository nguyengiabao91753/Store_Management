using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _iDashboardService;

        public HomeController(ILogger<HomeController> logger, IDashboardService iDashboardService)
        {
            _logger = logger;
            _iDashboardService = iDashboardService;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                MonthlyRevenue = _iDashboardService.GetMonthlyRevenue(),
                TopProducts = _iDashboardService.GetTopSellingProducts()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetOrders(DateTime? startDate, DateTime? endDate, int? month, int? year, int pageIndex = 1, int pageSize = 10)
        {
            var orders = _iDashboardService.GetRecentOrders(startDate, endDate, month, year, pageIndex, pageSize);
            var totalOrders = _iDashboardService.GetRecentOrders(startDate, endDate, month, year).Count;
            var totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);
             return Json(new { data = orders, totalPages, currentPage = pageIndex });
        }
    }
}
