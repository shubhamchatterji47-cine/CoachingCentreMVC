namespace CoachingMVC.ViewModels.Student
{
    public class SubjectPerformance
    {
        public string SubjectName { get; set; } = ""; public List<MarkViewModel> Marks { get; set; } = new();
        public double AveragePercentage { get; set; }
        public string Grade { get; set; } = "";
        public string Trend { get; set; } = ""; public int MaxMarks { get; set; }
    }
}
