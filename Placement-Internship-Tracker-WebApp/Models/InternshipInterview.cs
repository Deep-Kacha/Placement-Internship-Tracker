using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class InternshipInterview
    {
        public int Id { get; set; }
        public int InternshipApplicationId { get; set; }
        public InternshipApplication? InternshipApplication { get; set; }

        [Required]
        public string RoundType { get; set; } = "Technical";

        [Required, DataType(DataType.Date)] public DateTime InterviewDate { get; set; }
        [DataType(DataType.Time)] public TimeSpan? InterviewTime { get; set; }
        [Url] public string? MeetingLink { get; set; }
        public string? Venue { get; set; }
        public string? Notes { get; set; }
        public string? Outcome { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
