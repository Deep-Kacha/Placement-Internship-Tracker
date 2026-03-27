using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public string? Website { get; set; }
        public bool OffersInternships { get; set; } = true;
        public bool OffersPlacement { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
