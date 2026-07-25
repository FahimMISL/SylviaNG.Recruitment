using MediatR;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusDelete
{
    public class MaritalStatusDeleteCommand : IRequest<Unit>
    {
        public long MaritalStatusId { get; set; }

        public MaritalStatusDeleteCommand(long maritalStatusId)
        {
            MaritalStatusId = maritalStatusId;
        }
    }
}
