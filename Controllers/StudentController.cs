using Microsoft.AspNetCore.Mvc;
using CoachingMVC.Services;
using CoachingMVC.ViewModels.Student;

namespace CoachingMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApiService _api;
        public StudentController(ApiService api) { _api = api; }

        private IActionResult? CheckStudent()
        {
            if (HttpContext.Session.GetString("Role") != "Student")
                return RedirectToAction("Login", "Account");
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var guard = CheckStudent(); if (guard != null) return guard;
            var perfTask = _api.GetAsync<StudentPerformanceViewModel>("student/performance");
            var profileTask = _api.GetAsync<StudentViewModel>("student/profile");
            await Task.WhenAll(perfTask, profileTask);
            ViewBag.Profile = await profileTask;
            return View(await perfTask);
        }

        public async Task<IActionResult> Marks(string? examType)
        {
            var guard = CheckStudent(); if (guard != null) return guard;
            var ep = string.IsNullOrEmpty(examType) ? "student/marks" : $"student/marks?examType={examType}";
            var marks = await _api.GetAsync<List<MarkViewModel>>(ep) ?? new();
            ViewBag.ExamType = examType ?? "All";
            return View(marks);
        }

        public async Task<IActionResult> Performance()
        {
            var guard = CheckStudent(); if (guard != null) return guard;
            var perf = await _api.GetAsync<StudentPerformanceViewModel>("student/performance");
            return View(perf);
        }

        public async Task<IActionResult> Profile()
        {
            var guard = CheckStudent(); if (guard != null) return guard;
            // Load full profile data to display in the view
            var profile = await _api.GetAsync<StudentViewModel>("student/profile");
            return View(profile);
        }
    }
}