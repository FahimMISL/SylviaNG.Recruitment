using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeDelete
{
    public class DegreeDeleteCommand : IRequest<Unit>
    {
        public long DegreeId { get; set; }

        public DegreeDeleteCommand(long degreeId)
        {
            DegreeId = degreeId;
        }
    }
}
