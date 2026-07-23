using MediatR;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusUpdate
{
    public class MaritalStatusUpdateCommand : IRequest<Unit>
    {
        public long MaritalStatusId { get; set; }
        public MaritalStatusUpdateRequest Request { get; set; }

        public MaritalStatusUpdateCommand(long maritalStatusId, MaritalStatusUpdateRequest request)
        {
            MaritalStatusId = maritalStatusId;
            Request = request;
        }
    }
}
