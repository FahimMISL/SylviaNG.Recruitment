using MediatR;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;

namespace SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.DistrictGetByDivision
{
    public class DistrictGetByDivisionQuery : IRequest<List<DistrictResponse>>
    {
        public long DivisionId { get; set; }

        public DistrictGetByDivisionQuery(long divisionId)
        {
            DivisionId = divisionId;
        }
    }
}
