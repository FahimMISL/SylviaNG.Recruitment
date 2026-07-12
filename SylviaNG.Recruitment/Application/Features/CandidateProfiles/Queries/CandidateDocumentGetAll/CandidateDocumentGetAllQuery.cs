using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateDocumentGetAll
{
    public class CandidateDocumentGetAllQuery : IRequest<List<CandidateDocumentResponse>>
    {
    }
}
