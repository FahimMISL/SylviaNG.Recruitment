using MediatR;
using SylviaNG.Recruitment.Application.Features.Degrees.Models;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeCreate
{
    public class DegreeCreateCommand : IRequest<long>
    {
        public DegreeCreateRequest Request { get; set; }

        public DegreeCreateCommand(DegreeCreateRequest request)
        {
            Request = request;
        }
    }
}
