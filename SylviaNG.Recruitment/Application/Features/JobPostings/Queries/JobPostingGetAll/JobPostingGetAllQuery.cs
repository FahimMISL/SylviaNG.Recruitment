using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAll
{
    public class JobPostingGetAllQuery : IRequest<List<JobPostingResponse>>
    {
    }
}
