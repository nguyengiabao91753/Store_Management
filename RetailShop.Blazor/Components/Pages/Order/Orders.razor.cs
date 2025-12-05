using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Components.Pages.Order;

public partial class Orders
{
    [Inject] private IOrderService OrderService { get; set; }

    [Inject] private CustomerStateService CustomerStateService { get; set; } 
    private List<OrderDTO> allOrders = new();
    private List<OrderDTO> filteredOrders = new();
    private string selectedTab = "all";
    private string searchQuery = "";
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        if(CustomerStateService.IsAuthenticated == false)
        {
            Nav.NavigateTo("/login");
            return;
        }

        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        isLoading = true;
        await Task.Delay(500); // Simulate API call

        var rs = await OrderService.GetOrdersByCustomer(CustomerStateService.CurrentCustomer.CustomerId);
        var resultString = Convert.ToString(rs.Result) ?? "[]";
        allOrders = JsonConvert.DeserializeObject<List<OrderDTO>>(resultString) ?? new List<OrderDTO>();

        isLoading = false;
        FilterOrders();
    }

    private void SelectTab(string tab)
    {
        selectedTab = tab;
        FilterOrders();
    }

    private void FilterOrders()
    {
        var orders = selectedTab == "all"
            ? allOrders
            : allOrders.Where(o => o.Status?.ToLower() == selectedTab).ToList();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            orders = orders.Where(o =>
                o.OrderId.ToString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                o.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                o.Status?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }

        filteredOrders = orders.OrderByDescending(o => o.OrderDate).ToList();
    }

    private void ViewOrderDetail(int orderId)
    {
        // Navigation will be implemented
        Console.WriteLine($"[v0] Navigating to order detail: {orderId}");
         Nav.NavigateTo($"/orders/{orderId}");
    }
}
