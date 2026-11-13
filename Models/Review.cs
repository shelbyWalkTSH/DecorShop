namespace DecorShop.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public int Rating { get; set; }
        public bool IsApproved { get; set; } // Модератор ставит true
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}