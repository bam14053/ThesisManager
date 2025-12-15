namespace ThesisManager.ViewModels
{
    public class CreateThesisViewModel
    {
        // Student info
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string? ResidencyId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        
        // Thesis info
        public string TitleAr { get; set; } = string.Empty;
        public string? TitleEn { get; set; }
        public string DegreeType { get; set; } = "master";
        public int? TrackId { get; set; }
        public string? NewTrackName { get; set; } // For creating new track
        public int? SupervisorId { get; set; }
        public DateTime? DefenseDate { get; set; }
        public string? AbstractAr { get; set; }
        public string? AbstractEn { get; set; }
    }
}