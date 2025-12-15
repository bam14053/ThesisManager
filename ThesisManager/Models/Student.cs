using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    [Table("students")]
    public class Student
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("student_id")]
        public string StudentId { get; set; } = null!;

        [Column("residency_id")]
        public string? ResidencyId { get; set; }

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = null!;

        [Column("email")]
        public string? Email { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("tutor_id")]
        [ForeignKey(nameof(Tutor))]
        public int? TutorId { get; set; }
        public User? Tutor { get; set; }

        [Column("supervisor_id")]
        [ForeignKey(nameof(Supervisor))]
        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }

        [Required]
        [Column("current_step")]
        public int CurrentStep { get; set; } = 1;

        [Column("current_deadline")]
        public DateTime? CurrentDeadline { get; set; }

        [Required]
        [Column("enrollment_date")]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [Column("expected_graduation_date")]
        public DateTime? ExpectedGraduationDate { get; set; }

        [Column("actual_graduation_date")]
        public DateTime? ActualGraduationDate { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = "active";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Thesis? Thesis { get; set; }
        public ICollection<StepHistory>? StepHistories { get; set; }
    }
}