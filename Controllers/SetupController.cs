using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DecorShop.Data;
using Microsoft.AspNetCore.Identity;

namespace DecorShop.Controllers
{
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SetupController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Просто зайдите на этот адрес: http://localhost:5041/Setup
        public async Task<IActionResult> Index()
        {
            // 1. Ищем первого зарегистрированного пользователя
            var user = await _userManager.Users.FirstOrDefaultAsync();
            if (user == null) 
                return Content("Ошибка: В базе нет ни одного пользователя! Сначала зарегистрируйтесь на сайте (через кнопку Register).");

            // 2. Проверяем, есть ли роль Admin (на случай если база старая)
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            // 3. Выдаем роль Admin первому пользователю
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Content($"УСПЕХ! Пользователь {user.Email} назначен Администратором. Теперь можете зайти в /Admin");
            }
            
            return Content($"Пользователь {user.Email} уже является Администратором.");
        }
    }
}