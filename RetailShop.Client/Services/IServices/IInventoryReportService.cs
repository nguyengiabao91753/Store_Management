using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices
{
    public interface IInventoryReportService
    {
        Task<List<InventoryStatisticModel>> GetInventoryReport(DateTime? fromDate, DateTime? toDate);
    }
}
