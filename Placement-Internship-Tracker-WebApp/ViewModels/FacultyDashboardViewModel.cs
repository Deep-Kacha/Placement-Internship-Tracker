using PlacementTracker.Models;

namespace PlacementTracker.ViewModels
{
    public class FacultyDashboardViewModel
    {
        public List<StudentProgressItem> Students { get; set; } = new();
        public List<InterviewSchedule> UpcomingInterviews { get; set; } = new();
        public List<InternshipInterview> UpcomingInternshipInterviews { get; set; } = new();
        public int TotalStudents { get; set; }
        public int TotalApplications { get; set; }
        public int TotalOffers { get; set; }
        public int TotalInternships { get; set; }
        public int TotalInternshipSelected { get; set; }
        public int TotalFullTimeOffered { get; set; }
    }

    public class StudentProgressItem
    {
        public ApplicationUser Student { get; set; } = null!;
        public int TotalApps { get; set; }
        public int Offers { get; set; }
        public int Interviews { get; set; }
        public string LatestStatus { get; set; } = string.Empty;
        public int TotalInternships { get; set; }
        public int InternshipSelected { get; set; }
        public bool HasFullTimeOffer { get; set; }
        public bool HasOngoingInternship { get; set; }
    }
}
