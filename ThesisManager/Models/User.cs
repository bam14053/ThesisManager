// Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThesisManager.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = null!;

        [Column("phone")]
        public string? Phone { get; set; }

        [Required]
        [Column("role")]
        public string Role { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("require_password_reset")]
        public bool RequirePasswordReset { get; set; } = false;

        public ICollection<Student>? TutoredStudents { get; set; }
        public ICollection<Student>? SupervisedStudents { get; set; }
    }
}