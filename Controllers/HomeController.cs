using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ОБЯЗАТЕЛЬНО для Include
using DecorShop.Data;
using DecorShop.Models;

namespace DecorShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                // 1. Берем 4 популярных товара
                PopularProducts = _context.Products
                    .Where(p => p.IsPopular)
                    .Take(4)
                    .ToList(),

                // 2. Берем последние 3 одобренных отзыва
                LatestReviews = _context.Reviews
                    .Include(r => r.Product) 
                    .Where(r => r.IsApproved == true) 
                    .OrderByDescending(r => r.CreatedAt) 
                    .Take(3) 
                    .ToList()
            };

            return View(viewModel);
        }

        public IActionResult Catalog()
        {
            // ВАЖНО: Добавлено .Include(p => p.Reviews), чтобы отзывы выводились в каталоге!
            var allProducts = _context.Products
                .Include(p => p.Reviews)
                .ToList();
                
            return View(allProducts);
        }
    }
}