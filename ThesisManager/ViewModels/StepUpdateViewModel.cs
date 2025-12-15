// ViewModels/StepUpdateViewModel.cs
namespace ThesisManager.ViewModels
{
    public class StepUpdateViewModel
    {
        public int StudentId { get; set; }
        public int StepNumber { get; set; }
        public DateTime? NewDeadline { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Notes { get; set; }
    }
}