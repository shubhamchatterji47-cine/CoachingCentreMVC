namespace CoachingMVC.ViewModels.Shared
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int CategoryId { get; set; }   // ← added
        public string CategoryName { get; set; } = "";
        public decimal? Fee { get; set; }
        public int DurationMonths { get; set; }
        public int ClassCount { get; set; }
        public bool IsActive { get; set; }
    }
}