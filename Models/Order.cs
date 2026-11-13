using System.ComponentModel.DataAnnotations;

namespace DecorShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public string UserId { get; set; } = ""; // Кто сделал заказ
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public decimal TotalAmount { get; set; } // Общая сумма
        
        public string Status { get; set; } = "В обработке"; // Статус заказа
        
        public List<OrderItem> Items { get; set; } = new(); // Список товаров в заказе
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Цена на момент покупки
    }
}