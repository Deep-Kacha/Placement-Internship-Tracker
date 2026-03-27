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
    }
}
