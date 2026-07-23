using MediatR;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;

namespace SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.ThanaGetByDistrict
{
    public class ThanaGetByDistrictQuery : IRequest<List<ThanaResponse>>
    {
        public long DistrictId { get; set; }

        public ThanaGetByDistrictQuery(long districtId)
        {
            DistrictId = districtId;
        }
    }
}
