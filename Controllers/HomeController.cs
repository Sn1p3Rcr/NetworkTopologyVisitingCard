using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NetworkTopologyVisitingCard.Models;
using System.Diagnostics;

namespace NetworkTopologyVisitingCard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            // СЕССИИ: счетчик просмотров главной страницы
            var viewCount = HttpContext.Session.GetInt32("ViewCount") ?? 0;
            viewCount++;
            HttpContext.Session.SetInt32("ViewCount", viewCount);
            ViewData["ViewCount"] = viewCount;

            // MEMORY CACHE: общий счетчик посещений сайта
            var cacheKey = "TotalVisits";
            if (!_memoryCache.TryGetValue(cacheKey, out int totalVisits))
            {
                totalVisits = 0;
            }
            totalVisits++;
            _memoryCache.Set(cacheKey, totalVisits, TimeSpan.FromHours(1));
            ViewData["TotalVisits"] = totalVisits;

            // КУКИ: тема оформления
            var theme = Request.Cookies["Theme"] ?? "light";
            ViewData["Theme"] = theme;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Система управления проектами для разработчиков";
            
            // QUERY STRING: пример использования
            var message = Request.Query["customMessage"];
            if (!string.IsNullOrEmpty(message))
            {
                ViewData["CustomMessage"] = message;
            }

            return View();
        }

        public IActionResult Technologies()
        {
            ViewData["Title"] = "Технологии";
            return View();
        }

        public IActionResult Demo()
        {
            ViewData["Title"] = "Демо";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Контакты";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Политика конфиденциальности";
            return View();
        }

        public IActionResult Requirements()
        {
            ViewData["Title"] = "Техническое задание";
            return View();
        }

        // Действие для смены темы (куки)
        public IActionResult ChangeTheme(string theme)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                IsEssential = true
            };
            
            Response.Cookies.Append("Theme", theme, options);
            return RedirectToAction("Index");
        }

        // Действие для очистки сессии
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // Действие для сброса кэша
        public IActionResult ClearCache()
        {
            var cacheKey = "TotalVisits";
            _memoryCache.Remove(cacheKey);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(errorViewModel);
        }
    }
}