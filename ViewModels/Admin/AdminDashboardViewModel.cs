namespace CoachingMVC.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalClasses { get; set; }
        public List<AnnouncementViewModel> RecentAnnouncements { get; set; } = new();
    }
}
