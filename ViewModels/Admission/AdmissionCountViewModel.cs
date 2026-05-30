namespace CoachingMVC.ViewModels.Admission
{
    public class AdmissionCountViewModel
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Reviewed { get; set; }
    }
}
