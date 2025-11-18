using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    public class StudentGuide
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required] 
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("tel")]
        public string? Tel { get; set; }

        [Column("e_mail")]
        public string? Email { get; set; }

        public ICollection<DissertationDraft>? Drafts { get; set; }
    }
}
