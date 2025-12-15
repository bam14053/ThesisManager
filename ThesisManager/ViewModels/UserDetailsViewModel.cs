namespace ThesisManager.ViewModels
{
    public class UserDetailsViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Students under this user
        public List<StudentProgressViewModel> Students { get; set; } = new();
    }
}