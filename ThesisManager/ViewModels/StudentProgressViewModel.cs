namespace ThesisManager.ViewModels
{
    public class StudentProgressViewModel
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string? ResidencyId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? DegreeType { get; set; } // From thesis
        public string? TrackName { get; set; } // From thesis
        public int CurrentStep { get; set; }
        public string StepNameAr { get; set; } = string.Empty;
        public DateTime? CurrentDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TutorName { get; set; }
        public string? SupervisorName { get; set; }
        public string? IdeaTitle { get; set; }
        public string? ThesisTitle { get; set; }
        public string DeadlineStatus { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ExpectedGraduationDate { get; set; }
    }
}
