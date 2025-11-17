using Microsoft.AspNetCore.Mvc;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryReportService _reportService;

        public InventoryController(IInventoryReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var data = await _reportService.GetInventoryReport(fromDate, toDate);
            return View(data);
        }
    }
}
