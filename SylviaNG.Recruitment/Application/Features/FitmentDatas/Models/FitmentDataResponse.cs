namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Models
{
    public class FitmentDataResponse
    {
        public long FitmentDataId { get; set; }
        public long JobApplicationId { get; set; }

        public string Designation { get; set; } = string.Empty;
        public string? Grade { get; set; }
        public string? Location { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
    }
}
