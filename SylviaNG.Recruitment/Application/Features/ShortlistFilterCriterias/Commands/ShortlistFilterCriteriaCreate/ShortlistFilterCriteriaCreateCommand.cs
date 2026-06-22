using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaCreate
{
    public class ShortlistFilterCriteriaCreateCommand : IRequest<long>
    {
        public ShortlistFilterCriteriaCreateRequest Request { get; set; }

        public ShortlistFilterCriteriaCreateCommand(ShortlistFilterCriteriaCreateRequest request)
        {
            Request = request;
        }
    }
}
