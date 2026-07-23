using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscUpdate
{
    public class MajorSubjectSscHscUpdateCommand : IRequest<Unit>
    {
        public long MajorSubjectSscHscId { get; set; }
        public MajorSubjectSscHscUpdateRequest Request { get; set; }

        public MajorSubjectSscHscUpdateCommand(long majorSubjectSscHscId, MajorSubjectSscHscUpdateRequest request)
        {
            MajorSubjectSscHscId = majorSubjectSscHscId;
            Request = request;
        }
    }
}
