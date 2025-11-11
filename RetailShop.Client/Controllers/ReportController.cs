using Microsoft.AspNetCore.Mvc;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Controllers
{
    [Route("Report")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetReportBarChart")]
        public async Task<IActionResult> GetReportBarChart(DateTime from_date, DateTime to_date, string groupBy = "day")
        {
            var data = await _reportService.GetReports(from_date, to_date, groupBy);
            return Json(data);
        }

        [HttpPost("GetReportDetails")]
        public async Task<IActionResult> GetReportDetails([FromBody]DateTime date)
        {
            var data = await _reportService.GetReportDetails(date);
            return Json(data);
        }

        [HttpPost("GetReportLineChart")]
        public async Task<IActionResult> GetReportLineChart(string groupBy = "month")
        {
            DateTime from_date = new DateTime(2023, 1, 1);
            DateTime to_date = DateTime.Now;
            var data = await _reportService.GetReports(from_date, to_date, groupBy);
            return Json(data);
        }

        [HttpPost("GetValue")]
        public async Task<IActionResult> GetValue(string groupBy = "year")
        {
            DateTime to_date = DateTime.Now;
            DateTime from_date = new DateTime(to_date.Year, 1, 1);
            var data = await _reportService.GetReports(from_date, to_date, groupBy);
            return Json(data);
        }

        [HttpGet("BestSellingProducts")]
        public async Task<IActionResult> BestSellingProducts()
        {
            DateTime to_date = DateTime.Now;
            DateTime from_date = new DateTime(to_date.Year, 1, 1);
            var data = await _reportService.GetBestSellingPorducts(from_date, to_date);
            return Json(data);
        }

        [HttpGet("LoyalCustomers")]
        public async Task<IActionResult> LoyalCustomers()
        {
            DateTime to_date = DateTime.Now;
            DateTime from_date = new DateTime(to_date.Year, 1, 1);
            var data = await _reportService.GetLoyalCustomers(from_date, to_date);
            return Json(data);
        }
    }
}
