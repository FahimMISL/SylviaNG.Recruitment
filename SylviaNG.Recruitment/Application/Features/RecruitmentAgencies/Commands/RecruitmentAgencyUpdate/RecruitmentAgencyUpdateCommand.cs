using MediatR;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyUpdate
{
    public class RecruitmentAgencyUpdateCommand : IRequest<Unit>
    {
        public long RecruitmentAgencyId { get; set; }
        public RecruitmentAgencyUpdateRequest Request { get; set; }

        public RecruitmentAgencyUpdateCommand(long recruitmentAgencyId, RecruitmentAgencyUpdateRequest request)
        {
            RecruitmentAgencyId = recruitmentAgencyId;
            Request = request;
        }
    }
}
