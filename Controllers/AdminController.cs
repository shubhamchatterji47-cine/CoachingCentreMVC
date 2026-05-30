using CoachingMVC.Models;
using CoachingMVC.Services;
using CoachingMVC.ViewModels.Admin;
using CoachingMVC.ViewModels.Shared;
using CoachingMVC.ViewModels.Student;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoachingMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _api;
        public AdminController(ApiService api) { _api = api; }

        private IActionResult? CheckAdmin()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var dashboard = await _api.GetAsync<AdminDashboardViewModel>("admin/dashboard") ?? new();

            // Pending admissions count
            var admCounts = await _api.GetAsync<Newtonsoft.Json.Linq.JObject>("admission/count");
            ViewBag.PendingAdmissions = admCounts?["pending"]?.Value<int>() ?? 0;

            // Unread contact messages count
            var msgCounts = await _api.GetAsync<Newtonsoft.Json.Linq.JObject>("contact/count");
            ViewBag.UnreadMessages = msgCounts?["unread"]?.Value<int>() ?? 0;

            return View(dashboard);
        }
        // Contact Messages
        public async Task<IActionResult> Messages(string? status)
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var ep = string.IsNullOrEmpty(status) ? "contact" : $"contact?status={status}";
            var msgs = await _api.GetAsync<List<CoachingMVC.ViewModels.Contact.ContactMessageViewModel>>(ep) ?? new();
            var counts = await _api.GetAsync<CoachingMVC.ViewModels.Contact.ContactCountViewModel>("contact/count") ?? new();
            ViewBag.StatusFilter = status;
            ViewBag.Counts = counts;
            return View(msgs);
        }

        [HttpPost]
        public async Task<IActionResult> MarkMessageRead(int id)
        {
            await _api.PutAsync($"contact/{id}/read", new { });
            return RedirectToAction("Messages");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            await _api.DeleteAsync($"contact/{id}");
            TempData["Success"] = "Message deleted.";
            return RedirectToAction("Messages");
        }
        // ── TEACHERS ──────────────────────────────────────────────
        public async Task<IActionResult> Teachers()
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var teachers = await _api.GetAsync<List<TeacherViewModel>>("admin/teachers") ?? new();
            return View(teachers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(string fullName, string email, string password,
            string? phone, string? qualification, string? specialization, int experienceYears, string? bio)
        {
            var (success, msg) = await _api.PostAsync("admin/teachers", new
            {
                fullName,
                email,
                password,
                phoneNumber = phone,
                qualification,
                specialization,
                experienceYears,
                bio
            });
            TempData[success ? "Success" : "Error"] = success ? "Teacher added successfully." : msg;
            return RedirectToAction("Teachers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            await _api.DeleteAsync($"admin/teachers/{id}");
            TempData["Success"] = "Teacher deactivated.";
            return RedirectToAction("Teachers");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTeacher(int id)
        {
            var (success, msg) = await _api.PutAsync($"admin/teachers/{id}/toggle", new { });
            TempData[success ? "Success" : "Error"] = success ? "Teacher status updated." : msg;
            return RedirectToAction("Teachers");
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDeleteTeacher(int id)
        {
            var (success, msg) = await _api.DeleteAsync($"admin/teachers/{id}/permanent");
            TempData[success ? "Success" : "Error"] = success ? "Teacher permanently deleted." : msg;
            return RedirectToAction("Teachers");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStudent(int id)
        {
            var (success, msg) = await _api.PutAsync($"admin/students/{id}/toggle", new { });
            TempData[success ? "Success" : "Error"] = success ? "Student status updated." : msg;
            return RedirectToAction("Students");
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDeleteStudent(int id)
        {
            var (success, msg) = await _api.DeleteAsync($"admin/students/{id}/permanent");
            TempData[success ? "Success" : "Error"] = success ? "Student permanently deleted." : msg;
            return RedirectToAction("Students");
        }

        // ── COURSES ──────────────────────────────────────────────
        public async Task<IActionResult> Courses()
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var courses = await _api.GetAsync<List<CourseViewModel>>("admin/courses") ?? new();
            var categories = await _api.GetAsync<List<CategoryViewModel>>("admin/categories") ?? new();
            ViewBag.Categories = categories;
            return View(courses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(string name, string? description, int categoryId,
            decimal? fee, int durationMonths)
        {
            var (success, msg) = await _api.PostAsync("admin/courses", new
            {
                name,
                description,
                categoryId,
                fee,
                durationMonths
            });
            TempData[success ? "Success" : "Error"] = success ? "Course created successfully." : msg;
            return RedirectToAction("Courses");
        }

        // ── SUBJECTS ──────────────────────────────────────────────
        public async Task<IActionResult> Subjects()
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var subjects = await _api.GetAsync<List<SubjectViewModel>>("admin/subjects") ?? new();
            var courses = await _api.GetAsync<List<CourseViewModel>>("admin/courses") ?? new();
            ViewBag.Courses = courses;
            return View(subjects);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject(string name, string? description, int courseId,
            int maxMarks, int passingMarks)
        {
            var (success, msg) = await _api.PostAsync("admin/subjects", new
            {
                name,
                description,
                courseId,
                maxMarks,
                passingMarks
            });
            TempData[success ? "Success" : "Error"] = success ? "Subject created successfully." : msg;
            return RedirectToAction("Subjects");
        }

        // ── CLASSES ──────────────────────────────────────────────
        public async Task<IActionResult> Classes()
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var classes = await _api.GetAsync<List<ClassViewModel>>("admin/classes") ?? new();
            var courses = await _api.GetAsync<List<CourseViewModel>>("admin/courses") ?? new();
            ViewBag.Courses = courses;
            return View(classes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(
            string name,
            string? description,
            int courseId,
            string? schedule,
            string? timing,
            int maxCapacity,
            string startDate,      // receive as string to avoid parse issues
            string? endDate)
        {
            // Parse dates safely
            DateTime parsedStart = DateTime.TryParse(startDate, out var sd) ? sd : DateTime.Today;
            DateTime? parsedEnd = DateTime.TryParse(endDate, out var ed) ? ed : (DateTime?)null;

            var (success, msg) = await _api.PostAsync("admin/classes", new
            {
                name,
                description,
                courseId,
                schedule,
                timing,
                maxCapacity,
                startDate = parsedStart,
                endDate = parsedEnd
            });

            TempData[success ? "Success" : "Error"] = success ? $"Class '{name}' created successfully!" : $"Error: {msg}";
            return RedirectToAction("Classes");
        }

        // ── STUDENTS ──────────────────────────────────────────────
        public async Task<IActionResult> Students(int? classId)
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var endpoint = classId.HasValue ? $"admin/students?classId={classId}" : "admin/students";
            var students = await _api.GetAsync<List<StudentViewModel>>(endpoint) ?? new();
            var classes = await _api.GetAsync<List<ClassViewModel>>("admin/classes") ?? new();
            ViewBag.Classes = classes;
            ViewBag.SelectedClassId = classId;
            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(
            string fullName,
            string email,
            string password,
            string? phone,
            int classId,
            string? rollNumber,
            string? dateOfBirth,
            string? parentName,
            string? parentPhone,
            string? address)
        {
            DateTime? dob = DateTime.TryParse(dateOfBirth, out var d) ? d : (DateTime?)null;

            var (success, msg) = await _api.PostAsync("admin/students/add", new
            {
                fullName,
                email,
                password,
                phoneNumber = phone,
                classId,
                rollNumber,
                dateOfBirth = dob,
                parentName,
                parentPhone,
                address
            });

            TempData[success ? "Success" : "Error"] = success
                ? $"Student '{fullName}' added successfully!"
                : $"Error: {msg}";

            return RedirectToAction("Students");
        }

        // ── ASSIGN TEACHER ──────────────────────────────────────────────
        public async Task<IActionResult> AssignTeacher()
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var teachers = await _api.GetAsync<List<TeacherViewModel>>("admin/teachers") ?? new();
            var classes = await _api.GetAsync<List<ClassViewModel>>("admin/classes") ?? new();
            var subjects = await _api.GetAsync<List<SubjectViewModel>>("admin/subjects") ?? new();
            var assignments = await _api.GetAsync<List<AssignmentViewModel>>("admin/assignments") ?? new();
            ViewBag.Teachers = teachers;
            ViewBag.Classes = classes;
            ViewBag.Subjects = subjects;
            ViewBag.Assignments = assignments;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            var (success, msg) = await _api.DeleteAsync($"admin/assignments/{id}");
            TempData[success ? "Success" : "Error"] = success ? "Assignment removed." : msg;
            return RedirectToAction("AssignTeacher");
        }

        [HttpPost]
        public async Task<IActionResult> AssignTeacher(int classId, int subjectId, int teacherId)
        {
            var (success, msg) = await _api.PostAsync("admin/assign-teacher", new { classId, subjectId, teacherId });
            TempData[success ? "Success" : "Error"] = success ? "Teacher assigned successfully." : msg;
            return RedirectToAction("AssignTeacher");
        }

        // ── ANNOUNCEMENTS ──────────────────────────────────────────────
        public async Task<IActionResult> Announcements()
        {
            var guard = CheckAdmin(); if (guard != null) return guard;
            var list = await _api.GetAsync<List<AnnouncementViewModel>>("admin/announcements") ?? new();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(string title, string content, string? targetRole)
        {
            var (success, msg) = await _api.PostAsync("admin/announcements", new { title, content, targetRole });
            TempData[success ? "Success" : "Error"] = success ? "Announcement posted." : msg;
            return RedirectToAction("Announcements");
        }
        public IActionResult Profile()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Account");
            return View();
        }

        // ── DELETE COURSE ──────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var (success, msg) = await _api.DeleteAsync($"admin/courses/{id}");
            TempData[success ? "Success" : "Error"] = success ? "Course deactivated successfully." : msg;
            return RedirectToAction("Courses");
        }

        // ── DELETE SUBJECT ─────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var (success, msg) = await _api.DeleteAsync($"admin/subjects/{id}");
            TempData[success ? "Success" : "Error"] = success ? "Subject deleted successfully." : msg;
            return RedirectToAction("Subjects");
        }

        // ── DELETE CLASS ───────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var (success, msg) = await _api.DeleteAsync($"admin/classes/{id}");
            TempData[success ? "Success" : "Error"] = success ? "Class deactivated successfully." : msg;
            return RedirectToAction("Classes");
        }

        // ── UPDATE COURSE ──────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateCourse(int id, string name, string? description,
            int categoryId, decimal? fee, int durationMonths)
        {
            var (success, msg) = await _api.PutAsync($"admin/courses/{id}", new
            {
                name,
                description,
                categoryId,
                fee,
                durationMonths
            });
            TempData[success ? "Success" : "Error"] = success ? $"Course '{name}' updated." : msg;
            return RedirectToAction("Courses");
        }

        // ── UPDATE SUBJECT ─────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateSubject(int id, string name, string? description,
            int courseId, int maxMarks, int passingMarks)
        {
            var (success, msg) = await _api.PutAsync($"admin/subjects/{id}", new
            {
                name,
                description,
                courseId,
                maxMarks,
                passingMarks
            });
            TempData[success ? "Success" : "Error"] = success ? $"Subject '{name}' updated." : msg;
            return RedirectToAction("Subjects");
        }

        // ── UPDATE CLASS ───────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateClass(int id, string name, string? description,
            int courseId, string? schedule, string? timing, int maxCapacity,
            string startDate, string? endDate)
        {
            DateTime parsedStart = DateTime.TryParse(startDate, out var sd) ? sd : DateTime.Today;
            DateTime? parsedEnd = DateTime.TryParse(endDate, out var ed) ? ed : (DateTime?)null;

            var (success, msg) = await _api.PutAsync($"admin/classes/{id}", new
            {
                name,
                description,
                courseId,
                schedule,
                timing,
                maxCapacity,
                startDate = parsedStart,
                endDate = parsedEnd
            });
            TempData[success ? "Success" : "Error"] = success ? $"Class '{name}' updated." : msg;
            return RedirectToAction("Classes");
        }

        // ── UPDATE TEACHER ─────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateTeacher(int id, string fullName, string? phone,
            string? qualification, string? specialization, int experienceYears, string? bio)
        {
            var (success, msg) = await _api.PutAsync($"admin/teachers/{id}", new
            {
                fullName,
                phoneNumber = phone,
                qualification,
                specialization,
                experienceYears,
                bio
            });
            TempData[success ? "Success" : "Error"] = success ? $"Teacher '{fullName}' updated." : msg;
            return RedirectToAction("Teachers");
        }

        // ── UPDATE STUDENT ─────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateStudent(int id, string fullName, string? phone,
            int classId, string? rollNumber, string? parentName, string? parentPhone, string? address)
        {
            var (success, msg) = await _api.PutAsync($"teacher/students/{id}", new
            {
                fullName,
                phoneNumber = phone,
                classId,
                rollNumber,
                parentName,
                parentPhone,
                address
            });
            TempData[success ? "Success" : "Error"] = success ? $"Student '{fullName}' updated." : msg;
            return RedirectToAction("Students");
        }
        // ── DELETE ANNOUNCEMENT ────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var (success, msg) = await _api.DeleteAsync($"admin/announcements/{id}");
            TempData[success ? "Success" : "Error"] = success ? "Announcement deleted." : msg;
            return RedirectToAction("Announcements");
        }
    }
}