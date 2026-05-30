namespace CoachingMVC.ViewModels.Admin
{
    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = ""; public DateTime CreatedAt { get; set; }
        public string? TargetRole { get; set; }
    }
}
