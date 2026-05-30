using Microsoft.AspNetCore.Mvc;
using CoachingMVC.Services;
using CoachingMVC.ViewModels.Student;
using CoachingMVC.ViewModels.Shared;

namespace CoachingMVC.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApiService _api;
        public TeacherController(ApiService api) { _api = api; }

        private IActionResult? CheckTeacher()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Login", "Account");
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var guard = CheckTeacher(); if (guard != null) return guard;
            var classes = await _api.GetAsync<List<ClassViewModel>>("teacher/my-classes") ?? new();
            var totalStudents = 0;
            foreach (var cls in classes)
            {
                var students = await _api.GetAsync<List<StudentViewModel>>($"teacher/students/{cls.Id}") ?? new();
                totalStudents += students.Count;
            }
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalClasses = classes.Count;
            return View(classes);
        }

        public async Task<IActionResult> Students(int classId)
        {
            var guard = CheckTeacher(); if (guard != null) return guard;
            var classes = await _api.GetAsync<List<ClassViewModel>>("teacher/my-classes") ?? new();

            // Auto-select first class if none chosen
            if (classId == 0 && classes.Any())
                return RedirectToAction("Students", new { classId = classes.First().Id });

            var students = await _api.GetAsync<List<StudentViewModel>>($"teacher/students/{classId}") ?? new();
            ViewBag.Classes = classes;
            ViewBag.SelectedClassId = classId;
            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(string fullName, string email, string password,
            string? phone, int classId, string? rollNumber, string? parentName,
            string? parentPhone, string? address)
        {
            var (success, msg) = await _api.PostAsync("teacher/students", new
            {
                fullName,
                email,
                password,
                phoneNumber = phone,
                classId,
                rollNumber,
                parentName,
                parentPhone,
                address
            });
            TempData[success ? "Success" : "Error"] = success
                ? $"Student '{fullName}' added successfully!"
                : $"Error: {msg}";
            return RedirectToAction("Students", new { classId });
        }

        public async Task<IActionResult> Marks(int classId, string examType = "All")
        {
            var guard = CheckTeacher(); if (guard != null) return guard;

            var classes = await _api.GetAsync<List<ClassViewModel>>("teacher/my-classes") ?? new();

            // Auto-select first class if none chosen
            if (classId == 0 && classes.Any())
                return RedirectToAction("Marks", new { classId = classes.First().Id, examType });

            var marks = new List<MarkViewModel>();
            var students = new List<StudentViewModel>();
            var subjects = new List<SubjectViewModel>();

            if (classId > 0)
            {
                // Build marks URL — no examType filter when "All"
                var marksUrl = examType == "All"
                    ? $"teacher/marks/{classId}"
                    : $"teacher/marks/{classId}?examType={examType}";

                var marksTask = _api.GetAsync<List<MarkViewModel>>(marksUrl);
                var studentsTask = _api.GetAsync<List<StudentViewModel>>($"teacher/students/{classId}");
                var subjectsTask = _api.GetAsync<List<SubjectViewModel>>($"teacher/subjects/{classId}");

                await Task.WhenAll(marksTask, studentsTask, subjectsTask);

                marks = await marksTask ?? new();
                students = await studentsTask ?? new();
                subjects = await subjectsTask ?? new();
            }

            ViewBag.Classes = classes;
            ViewBag.Students = students;
            ViewBag.Subjects = subjects;
            ViewBag.SelectedClassId = classId;
            ViewBag.ExamType = examType;
            return View(marks);
        }

        [HttpPost]
        public async Task<IActionResult> AddMark(
            int studentId, int subjectId, decimal obtainedMarks,
            string examType, string? remarks, string examDate, int classId)
        {
            DateTime parsedDate = DateTime.TryParse(examDate, out var d) ? d : DateTime.Today;

            var (success, msg) = await _api.PostAsync("teacher/marks", new
            {
                studentId,
                subjectId,
                obtainedMarks,
                examType,
                remarks,
                examDate = parsedDate
            });

            TempData[success ? "Success" : "Error"] = success
                ? "Mark saved successfully!"
                : $"Error: {msg}";

            return RedirectToAction("Marks", new { classId, examType = "All" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMark(int id, int classId, string examType)
        {
            var (success, msg) = await _api.DeleteAsync($"teacher/marks/{id}");
            TempData[success ? "Success" : "Error"] = success ? "Mark deleted." : msg;
            return RedirectToAction("Marks", new { classId, examType });
        }

        public async Task<IActionResult> StudentPerformance(int studentId)
        {
            var guard = CheckTeacher(); if (guard != null) return guard;
            var perf = await _api.GetAsync<StudentPerformanceViewModel>($"student/performance/{studentId}");
            if (perf == null) return NotFound();
            return View(perf);
        }
        public async Task<IActionResult> Profile()
        {
            var guard = CheckTeacher(); if (guard != null) return guard;
            var teacherId = HttpContext.Session.GetInt32("RoleEntityId") ?? 0;
            if (teacherId > 0)
            {
                var teachers = await _api.GetAsync<List<CoachingMVC.ViewModels.Admin.TeacherViewModel>>("admin/teachers");
                ViewBag.Teacher = teachers?.FirstOrDefault(t => t.Id == teacherId);
            }
            return View();
        }
    }
}