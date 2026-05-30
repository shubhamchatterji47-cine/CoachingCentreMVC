namespace CoachingMVC.ViewModels.Student
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public int ClassId { get; set; }   // ← added (needed for edit modal)
        public string ClassName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string? RollNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }   // ← added
        public string? Address { get; set; }   // ← added
        public bool IsActive { get; set; }
    }
}