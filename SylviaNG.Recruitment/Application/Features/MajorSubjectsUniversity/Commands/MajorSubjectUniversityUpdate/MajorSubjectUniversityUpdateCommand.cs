using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityUpdate
{
    public class MajorSubjectUniversityUpdateCommand : IRequest<Unit>
    {
        public long MajorSubjectUniversityId { get; set; }
        public MajorSubjectUniversityUpdateRequest Request { get; set; }

        public MajorSubjectUniversityUpdateCommand(long majorSubjectUniversityId, MajorSubjectUniversityUpdateRequest request)
        {
            MajorSubjectUniversityId = majorSubjectUniversityId;
            Request = request;
        }
    }
}
