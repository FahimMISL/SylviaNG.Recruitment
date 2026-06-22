using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyCreate
{
    public class RecruitmentAgencyCreateCommand : IRequest<long>
    {
        public RecruitmentAgencyCreateRequest Request { get; set; }

        public RecruitmentAgencyCreateCommand(RecruitmentAgencyCreateRequest request)
        {
            Request = request;
        }
    }
}
