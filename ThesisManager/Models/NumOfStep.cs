using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    public class NumOfStep
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; } = null!;
        public ICollection<Student>? Students { get; set; }
    }
}
