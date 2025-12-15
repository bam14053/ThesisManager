// ViewModels/CreateUserViewModel.cs
namespace ThesisManager.ViewModels
{
    public class CreateUserViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } // Now optional - will be auto-generated if null
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}