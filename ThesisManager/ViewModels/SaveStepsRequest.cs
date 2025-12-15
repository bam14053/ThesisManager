// ViewModels/SaveStepsRequest.cs
namespace ThesisManager.ViewModels
{
    public class SaveStepsRequest
    {
        public List<UpdatedStepDto> UpdatedSteps { get; set; } = new();
        public List<NewStepDto> NewSteps { get; set; } = new();
    }

    public class UpdatedStepDto
    {
        public int StepNumber { get; set; }
        public string StepNameAr { get; set; } = string.Empty;
        public string? StepNameEn { get; set; }
        public int DefaultDurationDays { get; set; }
    }

    public class NewStepDto
    {
        public string StepNameAr { get; set; } = string.Empty;
        public string? StepNameEn { get; set; }
        public int DefaultDurationDays { get; set; }
    }
}