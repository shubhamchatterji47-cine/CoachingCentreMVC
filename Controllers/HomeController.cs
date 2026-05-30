using Microsoft.AspNetCore.Mvc;
using CoachingMVC.Services;
using CoachingMVC.ViewModels.Admin;
using CoachingMVC.ViewModels.Shared;

namespace CoachingMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _api;
        public HomeController(ApiService api) { _api = api; }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin") return RedirectToAction("Index", "Admin");
            if (role == "Teacher") return RedirectToAction("Index", "Teacher");
            if (role == "Student") return RedirectToAction("Index", "Student");

            var coursesTask = _api.GetAsync<List<CourseViewModel>>("public/courses");
            var categoriesTask = _api.GetAsync<List<CategoryViewModel>>("public/categories");
            var announcementsTask = _api.GetAsync<List<AnnouncementViewModel>>("public/announcements");
            await Task.WhenAll(coursesTask, categoriesTask, announcementsTask);

            ViewBag.Courses = (await coursesTask ?? new()).Take(6).ToList();
            ViewBag.Categories = await categoriesTask ?? new();
            ViewBag.Announcements = (await announcementsTask ?? new()).Take(3).ToList();
            return View();
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _api.GetAsync<List<CourseViewModel>>("public/courses") ?? new();
            var categories = await _api.GetAsync<List<CategoryViewModel>>("public/categories") ?? new();
            ViewBag.Categories = categories;
            return View(courses);
        }

        public async Task<IActionResult> Classes()
        {
            var classes = await _api.GetAsync<List<ClassViewModel>>("public/classes") ?? new();
            return View(classes);
        }

        public async Task<IActionResult> Contact()
        {
            var courses = await _api.GetAsync<List<CourseViewModel>>("public/courses") ?? new();
            ViewBag.Courses = courses;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(
            string name, string email, string? phone, string? subject, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(message))
            {
                TempData["ContactError"] = "Please fill in your name, email and message.";
                return RedirectToAction("Contact");
            }

            // ? correct endpoint: contact/send (not admission/apply)
            var (success, msg) = await _api.PostAsync("contact/send", new
            {
                name,
                email,
                phone,
                subject,
                message
            });

            TempData[success ? "ContactSuccess" : "ContactError"] = success
                ? "? Message sent! We'll get back to you soon."
                : (msg ?? "Something went wrong. Please try again.");

            return RedirectToAction("Contact");
        }

        [HttpGet]
        public async Task<IActionResult> GetAnnouncements()
        {
            var list = await _api.GetAsync<List<AnnouncementViewModel>>("public/announcements") ?? new();
            return Json(list);
        }

        public IActionResult About() => View();
        public IActionResult Privacy() => View();
        public IActionResult Error() => View();
    }
}