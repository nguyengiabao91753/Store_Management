using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices
{
    public interface IReportService
    {
        Task<List<Report>> GetReports(DateTime from_date, DateTime to_date, String groupBy = "day");

        Task<List<ReportDetail>> GetReportDetails(DateTime date);

        Task<List<Report>> GetBestSellingPorducts(DateTime from_date, DateTime to_date);

        Task<List<Report>> GetLoyalCustomers(DateTime from_date, DateTime to_date);
    }
}
