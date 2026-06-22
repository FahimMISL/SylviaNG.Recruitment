namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Models
{
    public class FitmentDataResponse
    {
        public long FitmentDataId { get; set; }
        public long CandidateId { get; set; }
        public long JobApplicationId { get; set; }
        public string? ProposedGrade { get; set; }
        public string? ProposedRole { get; set; }
        public string? Location { get; set; }
        public string? SalaryStructureJson { get; set; }
        public string? PayrollSource { get; set; }
        public bool IsFetchedFromPayroll { get; set; }
        public bool IsManualEntry { get; set; }
        public DateTime FetchedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
