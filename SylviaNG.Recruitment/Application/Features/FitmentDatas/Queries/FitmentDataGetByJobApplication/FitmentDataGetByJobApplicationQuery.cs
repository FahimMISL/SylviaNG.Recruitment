using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetByJobApplication
{
    public class FitmentDataGetByJobApplicationQuery : IRequest<FitmentDataResponse?>
    {
        public long JobApplicationId { get; set; }

        public FitmentDataGetByJobApplicationQuery(long jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}
