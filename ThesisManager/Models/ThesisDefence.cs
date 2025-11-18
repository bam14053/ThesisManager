using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    public class ThesisDefence
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("dissertation_id")]
        public int DissertationId { get; set; }
        public Dissertation? Dissertation { get; set; }
    }
}
