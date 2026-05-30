namespace CoachingMVC.ViewModels.Account
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = ""; public string Role { get; set; } = "";
        public string Token { get; set; } = ""; public int? RoleEntityId { get; set; }
    }
}
