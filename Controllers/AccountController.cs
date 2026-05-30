using Microsoft.AspNetCore.Mvc;
using CoachingMVC.Services;
using CoachingMVC.ViewModels.Account;
using Newtonsoft.Json;

namespace CoachingMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _api;
        public AccountController(ApiService api) { _api = api; }

        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
            {
                var role = HttpContext.Session.GetString("Role");
                return role switch
                {
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "Teacher" => RedirectToAction("Index", "Teacher"),
                    "Student" => RedirectToAction("Index", "Student"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message, data) = await _api.PostAsync<UserViewModel>("auth/login", new
            {
                email = model.Email,
                password = model.Password
            });

            if (!success || data == null)
            {
                ViewBag.Error = message ?? "Invalid email or password.";
                return View(model);
            }

            HttpContext.Session.SetString("Token", data.Token);
            HttpContext.Session.SetString("Role", data.Role);
            HttpContext.Session.SetString("FullName", data.FullName);
            HttpContext.Session.SetInt32("UserId", data.UserId);
            if (data.RoleEntityId.HasValue)
                HttpContext.Session.SetInt32("RoleEntityId", data.RoleEntityId.Value);

            TempData["Success"] = $"Welcome back, {data.FullName}!";

            return data.Role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Teacher" => RedirectToAction("Index", "Teacher"),
                "Student" => RedirectToAction("Index", "Student"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // ── CHANGE PASSWORD ──────────────────────────────────────────
        // Called via fetch from profile pages — routes through MVC so port is always correct
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.CurrentPassword) ||
                string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest(new { message = "All fields are required." });

            var (success, msg) = await _api.PostAsync("auth/change-password", new
            {
                currentPassword = req.CurrentPassword,
                newPassword = req.NewPassword
            });

            if (success)
                return Ok(new { message = "Password changed successfully." });

            return BadRequest(new { message = msg ?? "Failed to change password." });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }

    // Simple request model for change password
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}