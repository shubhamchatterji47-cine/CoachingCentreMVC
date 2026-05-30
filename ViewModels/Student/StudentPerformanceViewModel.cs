namespace CoachingMVC.ViewModels.Student
{
    public class StudentPerformanceViewModel
    {

        public StudentViewModel Student { get; set; } = null!;
        public List<SubjectPerformance> SubjectPerformances { get; set; } = new();
        public double OverallPercentage { get; set; }
        public string OverallGrade { get; set; } = "";
        public string PerformanceTrend { get; set; } = "";
        public List<string> ImprovementTips { get; set; } = new();
        public double AttendancePercentage { get; set; }


    }
}
