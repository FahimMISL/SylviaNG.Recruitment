using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IFitmentDataService
    {
        /// <summary>Null (not 404) when nothing has been configured yet for this application, so
        /// the frontend can distinguish "not configured" from an error.</summary>
        Task<FitmentDataResponse?> GetByJobApplicationIdAsync(long jobApplicationId);

        Task<FitmentDataResponse> UpsertAsync(FitmentDataUpsertRequest request);
    }
}
