using SylviaNG.Recruitment.Application.Common.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Externals
{
    public interface ICoreGrpcClient
    {
        Task<CoreBatchLookupResult> GetSitesAsync(List<long> siteIds);

        /// <summary>Resolves department/designation display names for internal candidate
        /// profiles (US-005 AC1) - CandidateProfile only stores the Core-HR ids.</summary>
        Task<CoreBatchLookupResult> GetDepartmentsAndDesignationsAsync(List<long> departmentIds, List<long> designationIds);
    }
}
