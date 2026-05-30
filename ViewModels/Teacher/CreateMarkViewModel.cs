namespace CoachingMVC.ViewModels.Teacher
{
    public class CreateMarkViewModel
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public decimal ObtainedMarks { get; set; }
        public string ExamType { get; set; } = "Test";
        public string? Remarks { get; set; }
        public DateTime ExamDate { get; set; } = DateTime.Today;
    }
}
