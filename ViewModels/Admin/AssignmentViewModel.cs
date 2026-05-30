namespace CoachingMVC.ViewModels.Admin
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = "";
        public int ClassId { get; set; }
        public string ClassName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
    }
}