using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Components.Pages.Order;

public partial class OrderDetail
{
    [Parameter]
    public int OrderId { get; set; }
    [Inject] private IOrderService OrderService { get; set; }
    [Inject] private CustomerStateService CustomerStateService { get; set; }
    private OrderDTO? order;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        if (CustomerStateService.IsAuthenticated == false)
        {
            Nav.NavigateTo("/login");
            return;
        }
        await LoadOrderDetail(OrderId);
    }

    private async Task LoadOrderDetail(int OrderId)
    {
        isLoading = true;
        await Task.Delay(500); // Simulate API call

        var rs = await OrderService.GetOrderById(OrderId);
        order = JsonConvert.DeserializeObject<OrderDTO>(Convert.ToString(rs.Result));
        

        isLoading = false;
    }

    private void ExportOrder()
    {
        Console.WriteLine($"[v0] Exporting order #{order?.OrderId} to PDF");
        // Implement PDF export functionality
        // You can use libraries like QuestPDF, iTextSharp, or DinkToPdf
    }
}
