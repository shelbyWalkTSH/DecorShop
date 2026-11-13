using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DecorShop.Data;
using DecorShop.Models;
using Microsoft.AspNetCore.Identity;

namespace DecorShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Добавить товар в корзину
        public async Task<IActionResult> Add(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" }); // Требуем вход

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ProductId == id && c.UserId == userId);
            if (cartItem == null)
            {
                _context.CartItems.Add(new CartItem { ProductId = id, UserId = userId, Quantity = 1 });
            }
            else
            {
                cartItem.Quantity++;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Посмотреть корзину
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var cart = await _context.CartItems.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();
            return View(cart);
        }

        // Оформление заказа (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            // 1. Получаем корзину пользователя
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Если корзина пуста - нечего оформлять
            if (!cartItems.Any()) return RedirectToAction("Index"); 

            // 2. Создаем новый заказ
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "В обработке",
                // Считаем общую сумму
                TotalAmount = cartItems.Sum(x => (x.Product?.Price ?? 0) * x.Quantity),
                Items = new List<OrderItem>()
            };

            // 3. Переносим товары из корзины в заказ
            foreach (var item in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product?.Price ?? 0
                });
            }

            // 4. Сохраняем заказ в БД
            _context.Orders.Add(order);

            // 5. Очищаем корзину пользователя
            _context.CartItems.RemoveRange(cartItems);
            
            await _context.SaveChangesAsync();

            // 6. Перенаправляем на страницу "Спасибо"
            return RedirectToAction("Success");
        }

        // Страница успешного оформления заказа
        public IActionResult Success()
        {
            return View();
        }
        // Удалить товар из корзины
public async Task<IActionResult> Remove(int id)
{
    var userId = _userManager.GetUserId(User);
    if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

    var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    if (cartItem != null)
    {
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction("Index");
}
    }
}