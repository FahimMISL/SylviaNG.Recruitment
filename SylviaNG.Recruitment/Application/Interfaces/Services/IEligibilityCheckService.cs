using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IEligibilityCheckService
    {
        (bool IsEligible, List<string> Reasons) CheckEligibility(Candidate candidate, JobPosting job);
    }
}
