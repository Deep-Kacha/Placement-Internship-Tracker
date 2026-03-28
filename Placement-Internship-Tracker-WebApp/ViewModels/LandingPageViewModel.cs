using PlacementTracker.Models;

namespace PlacementTracker.ViewModels
{
    public class LandingPageViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalPlaced { get; set; }
        public int TotalCompanies { get; set; }
        public string? RecentApplicantName { get; set; }
        public List<JobDescription> RecentJobs { get; set; } = new List<JobDescription>();
    }
}
