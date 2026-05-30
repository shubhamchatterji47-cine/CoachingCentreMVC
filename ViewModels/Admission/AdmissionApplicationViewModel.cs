namespace CoachingMVC.ViewModels.Admission
{
    public class AdmissionApplicationViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PreviousSchool { get; set; }
        public string? LastClassPassed { get; set; }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? CourseInterest { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminNotes { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
