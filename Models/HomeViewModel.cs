namespace DecorShop.Models
{
    public class HomeViewModel
    {
        // Список популярных товаров
        public List<Product> PopularProducts { get; set; } = new();
        
        // Список одобренных отзывов для главной
        public List<Review> LatestReviews { get; set; } = new();
    }
}