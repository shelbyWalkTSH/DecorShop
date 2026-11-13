using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DecorShop.Data;
using DecorShop.Models;
using Microsoft.AspNetCore.Identity;

namespace DecorShop.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Форма добавления отзыва
        public IActionResult Create(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            ViewBag.ProductName = product.Name;
            return View(new Review { ProductId = productId });
        }

        // POST: Сохранение отзыва
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (ModelState.IsValid)
            {
                // По умолчанию отзыв не одобрен (ждет модератора)
                review.IsApproved = false;
                review.CreatedAt = DateTime.Now;
                
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Thanks"); // Страница благодарности
            }
            return View(review);
        }

        // Страница "Спасибо за отзыв"
        public IActionResult Thanks()
        {
            return View();
        }
    }
}