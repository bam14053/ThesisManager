// Models/StepDefinition.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    [Table("step_definitions")]
    public class StepDefinition
    {
        [Key]
        [Column("step_number")]
        public int StepNumber { get; set; }

        [Required]
        [Column("step_name_ar")]
        public string StepNameAr { get; set; } = null!;

        [Column("step_name_en")]
        public string? StepNameEn { get; set; }

        [Column("default_duration_days")]
        public int? DefaultDurationDays { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        public ICollection<StepHistory>? StepHistories { get; set; }
    }
}