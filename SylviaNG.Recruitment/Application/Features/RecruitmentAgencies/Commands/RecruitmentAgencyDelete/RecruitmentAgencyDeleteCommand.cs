using MediatR;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyDelete
{
    public class RecruitmentAgencyDeleteCommand : IRequest<Unit>
    {
        public long RecruitmentAgencyId { get; set; }

        public RecruitmentAgencyDeleteCommand(long recruitmentAgencyId)
        {
            RecruitmentAgencyId = recruitmentAgencyId;
        }
    }
}
