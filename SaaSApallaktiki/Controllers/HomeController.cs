using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSApallaktiki.Models;
using System.Diagnostics;

namespace SaaSApallaktiki.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SaaSDBContext _context;

        public HomeController(ILogger<HomeController> logger, SaaSDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            TempData["NotFound"] = 0;
            return View();
        }

        public IActionResult Register()
        {
            //TempData["NotFound"] = false;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            if (ModelState.IsValid)
            {
                var user2 = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == user.Username && m.Password == user.Password);
                if (user2 == null)
                {
                    TempData["NotFound"] = 1;
                    return View(user);
                }
                TempData["NotFound"] = 0;
                
                return RedirectToAction("MainMenu", new { userId = user2.UserId });
            }
            TempData["NotFound"] = 2;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("UserId,Username,Password")] User user)
        {
            user.UserId = 0;
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();


                /*var user2 = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == user.Username && m.Password == user.Password);
                if (user2 == null)
                {
                    TempData["NotFound"] = 1;
                    return View(user);
                }
                TempData["NotFound"] = 0;*/

                return RedirectToAction("MainMenu", new { userId = user.UserId });
            }
            //TempData["NotFound"] = 2;
            return View(user);
        }

        public async Task<IActionResult> MainMenu(int userId)
        {
            TempData["userId"] = userId;

            //check for notifications
            var notifications = await _context.Notifications.Where(m => m.IsRead == false && m.UserId == userId).ToListAsync();
            if (notifications.Any())
            {
                TempData["newNotification"] = true;
            }
            else
            {
                TempData["newNotification"] = false;
            }

            //Do the usual
            var user2 = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == userId);
            if (user2 == null)
            {
                TempData["NotFound"] = 1;
                //return View(user2);
                return RedirectToAction("Index");
            }
            TempData["NotFound"] = 0;
            return View(user2);
        }

        /*public IActionResult ShowHobbies(int userId)
        {
            var user2 = _context.Users.FirstAsync(m => m.UserId == userId);
            var hobbies = user2.Result.Hobbies;
            //return View(hobbies);
            return RedirectToAction("Index", "Hobbies", new { hobbiess = hobbies, userrId = userId });
        }*/
    }
}
