using PlacementTracker.Models;

namespace PlacementTracker.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int Applied { get; set; }
        public int Screening { get; set; }
        public int Interview { get; set; }
        public int Offer { get; set; }
        public int Rejected { get; set; }
        public int Withdrawn { get; set; }

        public int TotalInternships { get; set; }
        public int InternshipApplied { get; set; }
        public int InternshipShortlisted { get; set; }
        public int InternshipSelected { get; set; }
        public int InternshipCompleted { get; set; }
        public int InternshipRejected { get; set; }
        public int FullTimeOffered { get; set; }
        public bool HasOngoingInternship { get; set; }

        public List<InterviewSchedule> UpcomingInterviews { get; set; } = new();
        public List<InternshipInterview> UpcomingInternshipInterviews { get; set; } = new();
        public List<JobApplication> RecentApplications { get; set; } = new();
        public List<InternshipApplication> RecentInternships { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
    }
}
