using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscCreate
{
    public class MajorSubjectSscHscCreateCommand : IRequest<long>
    {
        public MajorSubjectSscHscCreateRequest Request { get; set; }

        public MajorSubjectSscHscCreateCommand(MajorSubjectSscHscCreateRequest request)
        {
            Request = request;
        }
    }
}
