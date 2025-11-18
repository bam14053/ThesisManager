using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    public class Student
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required] 
        [Column("name")]
        public string Name { get; set; } = null!;
        [StringLength(20)] 
        [Column("s_id")]
        public string? S_Id { get; set; }
        [StringLength(20)] 
        [Column("iqaamah_id")]
        public string? Iqaamah_Id { get; set; }
        [Column("tel")]
        public string? Tel { get; set; }
        [Column("e_mail")]
        public string? Email { get; set; }

        // FK
        [ForeignKey(nameof(NumOfStep))]
        [Column("num_of_step_id")]
        public int NumOfStepId { get; set; }
        public NumOfStep? NumOfStep { get; set; }

        [Required] 
        [Column("deadline")]
        public DateTime Deadline { get; set; }

        public ICollection<DissertationDraft>? Drafts { get; set; }
        public ICollection<Dissertation>? Dissertations { get; set; }
    }
}
