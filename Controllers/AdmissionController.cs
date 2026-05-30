using Microsoft.AspNetCore.Mvc;
using CoachingMVC.Services;
using CoachingMVC.ViewModels.Admission;
using CoachingMVC.ViewModels.Shared;

namespace CoachingMVC.Controllers
{
    public class AdmissionController : Controller
    {
        private readonly ApiService _api;
        public AdmissionController(ApiService api) { _api = api; }

        // PUBLIC — Admission form page
        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            var courses = await _api.GetAsync<List<CourseViewModel>>("public/courses") ?? new();
            ViewBag.Courses = courses;
            return View();
        }

        // PUBLIC — Submit admission form
        [HttpPost]
        public async Task<IActionResult> Apply(
            string fullName, string email, string phoneNumber,
            string? parentName, string? parentPhone,
            string? dateOfBirth, string? address,
            string? previousSchool, string? lastClassPassed,
            int? courseId, string? courseInterest, string? message)
        {
            DateTime? dob = DateTime.TryParse(dateOfBirth, out var d) ? d : (DateTime?)null;

            var (success, msg) = await _api.PostAsync("admission/apply", new
            {
                fullName,
                email,
                phoneNumber,
                parentName,
                parentPhone,
                dateOfBirth = dob,
                address,
                previousSchool,
                lastClassPassed,
                courseId,
                courseInterest,
                message
            });

            if (success)
            {
                TempData["AdmissionSuccess"] = "true";
                return RedirectToAction("Success");
            }

            // If failed, reload form with error
            var courses = await _api.GetAsync<List<CourseViewModel>>("public/courses") ?? new();
            ViewBag.Courses = courses;
            ViewBag.FormError = msg ?? "Something went wrong. Please try again.";

            // Preserve form data
            ViewBag.FullName = fullName;
            ViewBag.Email = email;
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.ParentName = parentName;
            ViewBag.ParentPhone = parentPhone;
            ViewBag.Address = address;
            ViewBag.PreviousSchool = previousSchool;
            ViewBag.CourseInterest = courseInterest;
            ViewBag.Message = message;

            return View();
        }

        // PUBLIC — Success page
        public IActionResult Success()
        {
            if (TempData["AdmissionSuccess"]?.ToString() != "true")
                return RedirectToAction("Apply");
            return View();
        }

        // ADMIN — List all applications
        public async Task<IActionResult> Index(string? status)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            var endpoint = string.IsNullOrEmpty(status)
                ? "admission" : $"admission?status={status}";

            var apps = await _api.GetAsync<List<AdmissionApplicationViewModel>>(endpoint) ?? new();
            var counts = await _api.GetAsync<AdmissionCountViewModel>("admission/count") ?? new();

            ViewBag.StatusFilter = status;
            ViewBag.Counts = counts;
            return View(apps);
        }

        // ADMIN — View single application detail
        public async Task<IActionResult> Detail(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            var app = await _api.GetAsync<AdmissionApplicationViewModel>($"admission/{id}");
            if (app == null) return NotFound();
            return View(app);
        }

        // ADMIN — Update application status
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? adminNotes)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            var (success, msg) = await _api.PutAsync($"admission/{id}/status", new { status, adminNotes });
            TempData[success ? "Success" : "Error"] = success
                ? $"Application {status.ToLower()} successfully."
                : msg;

            return RedirectToAction("Detail", new { id });
        }

        // ADMIN — Delete application
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            await _api.DeleteAsync($"admission/{id}");
            TempData["Success"] = "Application deleted.";
            return RedirectToAction("Index");
        }
    }
}