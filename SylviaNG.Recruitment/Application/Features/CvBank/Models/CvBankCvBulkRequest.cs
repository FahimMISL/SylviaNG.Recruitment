namespace SylviaNG.Recruitment.Application.Features.CvBank.Models
{
    public class CvBankCvBulkRequest
    {
        public List<long> CandidateProfileIds { get; set; } = new();
    }
}
