using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityCreate
{
    public class MajorSubjectUniversityCreateCommand : IRequest<long>
    {
        public MajorSubjectUniversityCreateRequest Request { get; set; }

        public MajorSubjectUniversityCreateCommand(MajorSubjectUniversityCreateRequest request)
        {
            Request = request;
        }
    }
}
