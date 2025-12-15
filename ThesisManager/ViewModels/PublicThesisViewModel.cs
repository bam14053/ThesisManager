// ViewModels/PublicThesisViewModel.cs
namespace ThesisManager.ViewModels
{
    public class PublicThesisViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string DegreeType { get; set; } = string.Empty;
        public string? TrackName { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string? AbstractAr { get; set; }
        public string? SupervisorName { get; set; }
        public DateTime? DefenseDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public int? GraduationYear { get; set; }
    }
}