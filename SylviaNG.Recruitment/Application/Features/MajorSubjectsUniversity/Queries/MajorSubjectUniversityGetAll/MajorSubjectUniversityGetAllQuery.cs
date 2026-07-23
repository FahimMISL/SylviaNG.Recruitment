using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Queries.MajorSubjectUniversityGetAll
{
    public class MajorSubjectUniversityGetAllQuery : IRequest<List<MajorSubjectUniversityResponse>>
    {
    }
}
