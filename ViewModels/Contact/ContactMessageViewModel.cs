namespace CoachingMVC.ViewModels.Contact
{
    public class ContactMessageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; } = "";
        public bool IsRead { get; set; }
        public string Status { get; set; } = "New";
        public string? AdminReply { get; set; }
        public DateTime SentAt { get; set; }
    }
}
