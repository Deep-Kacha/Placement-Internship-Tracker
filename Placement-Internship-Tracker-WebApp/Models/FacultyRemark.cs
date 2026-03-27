using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class FacultyRemark
    {
        public int Id { get; set; }
        [Required] public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        public int? JobApplicationId { get; set; }
        public JobApplication? JobApplication { get; set; }

        public int? InternshipApplicationId { get; set; }
        public InternshipApplication? InternshipApplication { get; set; }

        [Required, StringLength(1000)] public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
