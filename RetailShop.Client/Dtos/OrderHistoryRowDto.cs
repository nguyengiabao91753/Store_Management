using System;

namespace RetailShop.Client.Dtos;

public class OrderHistoryRowDto
{
	public int OrderId { get; set; }
	public DateTime? OrderDate { get; set; }
	public string? CustomerName { get; set; }
	public string OrderStatus { get; set; } = "Pending";
	public decimal TotalPayment { get; set; }
	public string PaymentStatus { get; set; } = "Unpaid";
}


