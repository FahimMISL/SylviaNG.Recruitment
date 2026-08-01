using MediatR;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityDelete
{
    public class MajorSubjectUniversityDeleteCommand : IRequest<Unit>
    {
        public long MajorSubjectUniversityId { get; set; }

        public MajorSubjectUniversityDeleteCommand(long majorSubjectUniversityId)
        {
            MajorSubjectUniversityId = majorSubjectUniversityId;
        }
    }
}
