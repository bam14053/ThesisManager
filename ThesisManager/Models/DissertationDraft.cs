using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    public class DissertationDraft
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required] 
        [Column("title")]
        public string Title { get; set; } = null!;
        [Column("student_id")]
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        [Column("student_guide_id")]
        public int StudentGuideId { get; set; }
        public StudentGuide? StudentGuide { get; set; }
        [Column("submission_date")]
        [Required] 
        public DateTime SubmissionDate { get; set; }
        [Column("is_approved")]
        public bool? IsApproved { get; set; }
    }
}
