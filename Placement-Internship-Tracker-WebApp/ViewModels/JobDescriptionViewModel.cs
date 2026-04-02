using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.ViewModels
{
    public class JobDescriptionViewModel
    {
        [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
        
        [Required, StringLength(150), Display(Name = "Company Name")] 
        public string CompanyName { get; set; } = string.Empty;
        
        [Required] public string Description { get; set; } = string.Empty;
        
        public string? Requirements { get; set; }

        [Required, Display(Name = "Type")]
        public string JobType { get; set; } = "Placement";

        [Required, DataType(DataType.Date)] 
        public DateTime Deadline { get; set; } = DateTime.Today.AddDays(15);

        public string? Location { get; set; }

        [Display(Name = "Package (LPA) or Stipend/Month")]
        public string? PackageOrStipend { get; set; }

        public string? AnnualCTC { get; set; }
        public string? Bond { get; set; }
        public string? SelectionProcess { get; set; }

        public string? CompanyWebsite { get; set; } 
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactMobile { get; set; }
        public string? InternshipDuration { get; set; }
        public string? Stipend { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string? AdditionalDetails { get; set; }
        
        [Display(Name = "Job Description (PDF/Word)")]
        public IFormFile? Document { get; set; }
        
        // Multi-select helpers (mapped to CSV)
        public List<string> SelectedBatches { get; set; } = new();
        public List<string> SelectedCourses { get; set; } = new();
    }
}
