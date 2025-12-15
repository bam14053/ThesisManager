// ViewModels/StudentDetailsViewModel.cs
using ThesisManager.Models;

namespace ThesisManager.ViewModels
{
    public class StudentDetailsViewModel
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string? ResidencyId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int CurrentStep { get; set; }
        public DateTime? CurrentDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ExpectedGraduationDate { get; set; }
        
        public int? TutorId { get; set; }
        public string? TutorName { get; set; }
        public int? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        
        public ThesisDetailsViewModel? Thesis { get; set; }
        public List<StepHistoryViewModel> StepHistory { get; set; } = new();
        public List<StepDefinition> AllSteps { get; set; } = new();
    }

    public class ThesisDetailsViewModel
    {
        public int Id { get; set; }
        public string DegreeType { get; set; } = string.Empty;
        public int? TrackId { get; set; }
        public string? TrackName { get; set; }
        public string? IdeaTitle { get; set; }
        public string? IdeaDescription { get; set; }
        public DateTime? IdeaSubmissionDate { get; set; }
        public DateTime? IdeaApprovalDate { get; set; }
        public string? IdeaDenialReason { get; set; }
        public string? ResearchPlan { get; set; }
        public DateTime? PlanSubmissionDate { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string? AbstractAr { get; set; }
        public string? AbstractEn { get; set; }
        public List<string>? DefenseCommittee { get; set; }
        public DateTime? CommitteeAssignmentDate { get; set; }
        public DateTime? DefenseDate { get; set; }
        public TimeSpan? DefenseTime { get; set; }
        public DateTime? CommitteeReadingDeadline { get; set; }
        public DateTime? DefenseCompletionDate { get; set; }
        public DateTime? CorrectionDeadline { get; set; }
        public DateTime? CorrectedSubmissionDate { get; set; }
        public DateTime? FinalSubmissionDate { get; set; }
    }

    public class StepHistoryViewModel
    {
        public int StepNumber { get; set; }
        public string StepNameAr { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime OriginalDeadline { get; set; }
        public DateTime CurrentDeadline { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int? DaysToComplete { get; set; }
        public bool WasLate { get; set; }
    }
}
