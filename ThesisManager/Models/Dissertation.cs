using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    public class Dissertation
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
        [Column("supervisor_id")]
        public int SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }
        [Column("submission_date")]
        [Required] public DateTime SubmissionDate { get; set; }

        public ThesisDefence? Defence { get; set; }
    }
}
