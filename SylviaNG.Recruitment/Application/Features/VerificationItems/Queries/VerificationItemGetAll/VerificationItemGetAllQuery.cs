using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetAll
{
    public class VerificationItemGetAllQuery : IRequest<List<VerificationItemResponse>>
    {
    }
}
