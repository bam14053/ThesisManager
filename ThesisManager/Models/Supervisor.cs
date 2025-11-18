using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    public class Supervisor
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required] 
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("tel")]
        public string? Tel { get; set; }

        // column name contains a hyphen in requirement -> map it
        [Column("e_mail")]
        public string? Email { get; set; }
        public ICollection<Dissertation>? Dissertations { get; set; }
    }
}
