namespace Shop.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string PaymentMethod { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    
    // Связи
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public List<OrderItem> OrderItems { get; set; } = new();
    
    // Методы для работы с заказом
    public void CalculateTotal()
    {
        TotalAmount = OrderItems.Sum(oi => oi.TotalPrice);
    }
}