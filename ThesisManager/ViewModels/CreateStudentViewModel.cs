
// ViewModels/CreateStudentViewModel.cs (SIMPLIFIED - no degree_type, no track, no enrollment_date input)
namespace ThesisManager.ViewModels
{
    public class CreateStudentViewModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string? ResidencyId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? TutorId { get; set; }
    }
}