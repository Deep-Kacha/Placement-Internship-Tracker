namespace PlacementTracker.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalFaculty { get; set; }
        public int TotalApplications { get; set; }
        public int TotalOffers { get; set; }
        public int TotalInterviews { get; set; }
        public double PlacementRate { get; set; }

        public int TotalInternships { get; set; }
        public int TotalInternshipSelected { get; set; }
        public int TotalInternshipCompleted { get; set; }
        public int TotalPPOConverted { get; set; }
        public double InternshipConversionRate { get; set; }

        public Dictionary<string, int> DepartmentWiseOffers { get; set; } = new();
        public Dictionary<string, int> StatusDistribution { get; set; } = new();
        public Dictionary<string, int> InternshipStatusDistribution { get; set; } = new();
        public Dictionary<string, int> TopCompanies { get; set; } = new();
        public Dictionary<string, int> TopInternshipCompanies { get; set; } = new();
        public Dictionary<string, int> InternshipTypeDistribution { get; set; } = new();

        public List<RecentActivityItem> RecentActivity { get; set; } = new();
        public List<RecentInternshipActivityItem> RecentInternshipActivity { get; set; } = new();
    }

    public class RecentActivityItem
    {
        public string StudentName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

    public class RecentInternshipActivityItem
    {
        public string StudentName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string InternshipType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
