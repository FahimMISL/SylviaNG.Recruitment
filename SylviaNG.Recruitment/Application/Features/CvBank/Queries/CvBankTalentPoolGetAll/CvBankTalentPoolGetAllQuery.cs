using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankTalentPoolGetAll
{
    public class CvBankTalentPoolGetAllQuery : IRequest<List<CvBankTalentPoolEntryResponse>>
    {
    }
}
