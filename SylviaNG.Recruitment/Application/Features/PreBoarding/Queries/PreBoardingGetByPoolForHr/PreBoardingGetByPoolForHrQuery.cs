using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Queries.PreBoardingGetByPoolForHr
{
    public class PreBoardingGetByPoolForHrQuery : IRequest<PreBoardingSubmissionResponse>
    {
        public long FinalSelectionPoolId { get; set; }

        public PreBoardingGetByPoolForHrQuery(long finalSelectionPoolId)
        {
            FinalSelectionPoolId = finalSelectionPoolId;
        }
    }
}
