using RetailShop.Models;

namespace RetailShop.Services.IServices
{
    public interface IInventoryReportService
    {
        Task<List<InventoryStatisticModel>> GetInventoryReport(DateTime? fromDate, DateTime? toDate);
    }
}
