using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetAll
{
    public class VerificationWorkflowGetAllQuery : IRequest<List<VerificationWorkflowResponse>>
    {
    }
}
