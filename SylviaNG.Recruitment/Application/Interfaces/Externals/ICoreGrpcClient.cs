using SylviaNG.Recruitment.Application.Common.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Externals
{
    public interface ICoreGrpcClient
    {
        Task<CoreBatchLookupResult> GetSitesAsync(List<long> siteIds);
    }
}
