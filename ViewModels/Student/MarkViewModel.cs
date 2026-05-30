namespace CoachingMVC.ViewModels.Student
{
    public class MarkViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = "";
        public string SubjectName { get; set; } = ""; public decimal ObtainedMarks { get; set; }
        public int MaxMarks { get; set; }
        public string ExamType { get; set; } = "";
        public string? Remarks { get; set; }
        public DateTime ExamDate { get; set; }
        public double Percentage { get; set; }
        public string Grade { get; set; } = "";
    }
}
