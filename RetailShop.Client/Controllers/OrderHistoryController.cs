using Microsoft.AspNetCore.Mvc;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Controllers;

    public class OrderHistoryController : Controller
    {
        private readonly IOrderPOSService _orderPOSService;

        public OrderHistoryController(IOrderPOSService orderPOSService)
        {
            _orderPOSService = orderPOSService;
        }

        public async Task<IActionResult> Index()
        {
			ViewBag.orders = await _orderPOSService.GetOrderHistoryRowsAsync();
            return View();
        }

		[HttpGet]
		public async Task<IActionResult> GetOrders(DateTime? startDate, DateTime? endDate, string? startTime, string? endTime)
		{
			DateTime? start = null;
			DateTime? end = null;

			bool hasStartTime = !string.IsNullOrWhiteSpace(startTime);
			bool hasEndTime = !string.IsNullOrWhiteSpace(endTime);

			// If user provided time without date, assume today
			if (!startDate.HasValue && hasStartTime)
			{
				startDate = DateTime.Today;
			}
			if (!endDate.HasValue && hasEndTime)
			{
				endDate = DateTime.Today;
			}

			if (startDate.HasValue)
			{
				TimeSpan t = TimeSpan.Zero;
				if (hasStartTime && TimeSpan.TryParse(startTime, out var parsedStart))
				{
					t = parsedStart;
				}
				start = startDate.Value.Date.Add(t);
			}

			if (endDate.HasValue)
			{
				TimeSpan t = new TimeSpan(23, 59, 59);
				if (hasEndTime && TimeSpan.TryParse(endTime, out var parsedEnd))
				{
					t = parsedEnd;
				}
				end = endDate.Value.Date.Add(t);
			}

			// If only one bound was provided, set reasonable defaults
			if (start.HasValue && !end.HasValue)
			{
				end = start.Value.Date.Add(new TimeSpan(23, 59, 59));
			}
			if (!start.HasValue && end.HasValue)
			{
				start = end.Value.Date; // midnight of end date
			}

			// Ensure start <= end
			if (start.HasValue && end.HasValue && start > end)
			{
				var tmp = start;
				start = end;
				end = tmp;
			}

			var rows = await _orderPOSService.GetOrderHistoryRowsAsync(start, end);
			return Json(rows);
		}

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderPOSService.GetOrderByIdWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
    }
}
