using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetMyPostings
{
    public class JobPostingGetMyPostingsQuery : IRequest<List<JobPostingResponse>>
    {
    }
}
