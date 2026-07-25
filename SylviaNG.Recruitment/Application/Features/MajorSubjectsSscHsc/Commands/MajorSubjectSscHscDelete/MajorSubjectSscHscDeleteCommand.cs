using MediatR;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscDelete
{
    public class MajorSubjectSscHscDeleteCommand : IRequest<Unit>
    {
        public long MajorSubjectSscHscId { get; set; }

        public MajorSubjectSscHscDeleteCommand(long majorSubjectSscHscId)
        {
            MajorSubjectSscHscId = majorSubjectSscHscId;
        }
    }
}
