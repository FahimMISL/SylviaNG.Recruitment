using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetAll
{
    public class CandidateComplaintGetAllQuery : IRequest<List<CandidateComplaintResponse>>
    {
    }
}
