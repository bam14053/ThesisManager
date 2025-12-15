// Models/StepHistory.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    [Table("step_history")]
    public class StepHistory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("student_id")]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        [Column("step_number")]
        public int StepNumber { get; set; }
        public StepDefinition? StepDefinition { get; set; }

        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("original_deadline")]
        public DateTime OriginalDeadline { get; set; }

        [Required]
        [Column("current_deadline")]
        public DateTime CurrentDeadline { get; set; }

        [Column("completion_date")]
        public DateTime? CompletionDate { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = "in_progress"; // pending, in_progress, completed, extended, denied

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
