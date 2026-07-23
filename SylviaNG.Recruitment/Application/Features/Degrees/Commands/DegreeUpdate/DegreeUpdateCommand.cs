using MediatR;
using SylviaNG.Recruitment.Application.Features.Degrees.Models;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeUpdate
{
    public class DegreeUpdateCommand : IRequest<Unit>
    {
        public long DegreeId { get; set; }
        public DegreeUpdateRequest Request { get; set; }

        public DegreeUpdateCommand(long degreeId, DegreeUpdateRequest request)
        {
            DegreeId = degreeId;
            Request = request;
        }
    }
}
