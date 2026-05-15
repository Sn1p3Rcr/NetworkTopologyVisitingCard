using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace NetworkTopologyVisitingCard.Controllers
{
    public class DemoController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public DemoController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // Главная демо-страница
        public IActionResult Index()
        {
            var demoData = new DemoBrowserData();

            // === ДЕМОНСТРАЦИЯ СЕССИЙ ===
            // Получаем или инициализируем данные сессии
            var sessionVisits = HttpContext.Session.GetInt32("DemoSessionVisits") ?? 0;
            sessionVisits++;
            HttpContext.Session.SetInt32("DemoSessionVisits", sessionVisits);

            var sessionUserData = HttpContext.Session.GetString("DemoSessionUserData") ?? "Нет данных";
            demoData.SessionVisits = sessionVisits;
            demoData.SessionUserData = sessionUserData;

            // === ДЕМОНСТРАЦИЯ КЭША ===
            var cacheKey = "DemoMemoryCache_Counter";
            if (!_memoryCache.TryGetValue(cacheKey, out int cacheCounter))
            {
                cacheCounter = 0;
            }
            cacheCounter++;
            _memoryCache.Set(cacheKey, cacheCounter, TimeSpan.FromHours(1));
            demoData.CacheCounter = cacheCounter;

            // === ДЕМОНСТРАЦИЯ КУК ===
            var userTheme = Request.Cookies["DemoTheme"] ?? "light";
            var userLanguage = Request.Cookies["DemoLanguage"] ?? "ru";
            demoData.Theme = userTheme;
            demoData.Language = userLanguage;

            return View(demoData);
        }

        // Сохранить данные в сессию
        [HttpPost]
        public IActionResult SetSessionData(string userData)
        {
            HttpContext.Session.SetString("DemoSessionUserData", userData);
            return RedirectToAction("Index");
        }

        // Очистить сессию
        public IActionResult ClearSessionDemo()
        {
            HttpContext.Session.Remove("DemoSessionVisits");
            HttpContext.Session.Remove("DemoSessionUserData");
            return RedirectToAction("Index");
        }

        // Сбросить кэш
        public IActionResult ClearCacheDemo()
        {
            _memoryCache.Remove("DemoMemoryCache_Counter");
            return RedirectToAction("Index");
        }

        // Установить куку для темы
        public IActionResult SetTheme(string theme)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(365),
                IsEssential = true,
                HttpOnly = false // Доступна для JavaScript
            };
            Response.Cookies.Append("DemoTheme", theme, options);
            return RedirectToAction("Index");
        }

        // Установить куку для языка
        public IActionResult SetLanguage(string language)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(365),
                IsEssential = true,
                HttpOnly = false
            };
            Response.Cookies.Append("DemoLanguage", language, options);
            return RedirectToAction("Index");
        }

        // Удалить все куки
        public IActionResult ClearCookiesDemo()
        {
            Response.Cookies.Delete("DemoTheme");
            Response.Cookies.Delete("DemoLanguage");
            return RedirectToAction("Index");
        }

        // Информационная страница с объяснением механизмов
        public IActionResult Info()
        {
            return View();
        }

        // API для получения информации в JSON (для AJAX примеров)
        [HttpGet]
        public IActionResult GetInfo()
        {
            var sessionVisits = HttpContext.Session.GetInt32("DemoSessionVisits") ?? 0;
            var theme = Request.Cookies["DemoTheme"] ?? "light";

            return Json(new
            {
                sessionVisits = sessionVisits,
                theme = theme,
                timestamp = DateTime.Now,
                message = "Данные получены через AJAX"
            });
        }
    }

    public class DemoBrowserData
    {
        // Сессии
        public int SessionVisits { get; set; }
        public string? SessionUserData { get; set; }

        // Кэш
        public int CacheCounter { get; set; }

        // Куки
        public string? Theme { get; set; }
        public string? Language { get; set; }
    }
}
