namespace Shop.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }  // Количество одного конкретного товара
    public decimal UnitPrice { get; set; } //Цена на момент заказа 
    public decimal Discount { get; set; }  // Скидка на ЭТОТ товар
    public decimal TotalPrice => Quantity * UnitPrice;
    
    // Связи многие-ко-многим через Order
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}