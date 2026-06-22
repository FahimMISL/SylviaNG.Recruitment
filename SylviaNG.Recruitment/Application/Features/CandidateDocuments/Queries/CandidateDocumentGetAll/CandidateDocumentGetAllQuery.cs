using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetAll
{
    public class CandidateDocumentGetAllQuery : IRequest<List<CandidateDocumentResponse>>
    {
    }
}
