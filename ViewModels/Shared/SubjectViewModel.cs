namespace CoachingMVC.ViewModels.Shared
{
    public class SubjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }   // ← added
        public string CourseName { get; set; } = "";
        public int CourseId { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
    }
}