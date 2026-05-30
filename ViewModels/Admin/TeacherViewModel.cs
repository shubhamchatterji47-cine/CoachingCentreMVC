namespace CoachingMVC.ViewModels.Admin
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? Qualification { get; set; }
        public string? Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
    }
}