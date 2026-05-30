namespace CoachingMVC.ViewModels.Shared
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int CourseId { get; set; }         // ADDED — needed for subject lookup
        public string CourseName { get; set; } = "";
        public string? Schedule { get; set; }
        public string? Timing { get; set; }
        public int MaxCapacity { get; set; }
        public int EnrolledCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
