// Models/Thesis.cs (NOW includes degree_type and track_id)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ThesisManager.Models
{
    [Table("theses")]
    public class Thesis
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("student_id")]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // NOW HERE: Degree type and track
        [Required]
        [Column("degree_type")]
        public string DegreeType { get; set; } = null!;

        [Column("track_id")]
        [ForeignKey(nameof(Track))]
        public int? TrackId { get; set; }
        public Track? Track { get; set; }

        // Step 2: Research Idea
        [Column("idea_title")]
        public string? IdeaTitle { get; set; }

        [Column("idea_description")]
        public string? IdeaDescription { get; set; }

        [Column("idea_submission_date")]
        public DateTime? IdeaSubmissionDate { get; set; }

        [Column("idea_approval_date")]
        public DateTime? IdeaApprovalDate { get; set; }

        [Column("idea_denial_reason")]
        public string? IdeaDenialReason { get; set; }

        // Step 3: Research Plan
        [Column("research_plan")]
        public string? ResearchPlan { get; set; }

        [Column("plan_submission_date")]
        public DateTime? PlanSubmissionDate { get; set; }

        // Final thesis
        [Column("title_ar")]
        public string? TitleAr { get; set; }

        [Column("title_en")]
        public string? TitleEn { get; set; }

        [Column("abstract_ar")]
        public string? AbstractAr { get; set; }

        [Column("abstract_en")]
        public string? AbstractEn { get; set; }

        [Column("keywords_ar")]
        public string? KeywordsAr { get; set; }

        [Column("keywords_en")]
        public string? KeywordsEn { get; set; }

        // Step 7: Defense Committee
        [Column("defense_committee", TypeName = "jsonb")]
        public JsonDocument? DefenseCommittee { get; set; }

        [Column("committee_assignment_date")]
        public DateTime? CommitteeAssignmentDate { get; set; }

        // Step 8: Defense approval
        [Column("defense_date")]
        public DateTime? DefenseDate { get; set; }

        [Column("defense_time")]
        public TimeSpan? DefenseTime { get; set; }

        [Column("committee_reading_deadline")]
        public DateTime? CommitteeReadingDeadline { get; set; }

        // Step 9: Defense completion
        [Column("defense_completion_date")]
        public DateTime? DefenseCompletionDate { get; set; }

        [Column("correction_deadline")]
        public DateTime? CorrectionDeadline { get; set; }

        // Step 10-11: Final submission
        [Column("corrected_submission_date")]
        public DateTime? CorrectedSubmissionDate { get; set; }

        [Column("final_submission_date")]
        public DateTime? FinalSubmissionDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}