using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.DistrictGetByDivision;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.DivisionGetAll;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.ThanaGetByDistrict;

namespace SylviaNG.Recruitment.Controllers
{
    // No class-level [Authorize] - global AuthorizeFilter already requires login, and this is a
    // read-only lookup any authenticated role (candidate filling the address form) may hit.
    [ApiController]
    [Route("recruitment/address-lookup")]
    public class AddressLookupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressLookupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("divisions")]
        public async Task<ActionResult<List<DivisionResponse>>> GetDivisions()
        {
            var result = await _mediator.Send(new DivisionGetAllQuery());
            return Ok(result);
        }

        [HttpGet("districts")]
        public async Task<ActionResult<List<DistrictResponse>>> GetDistricts([FromQuery] long divisionId)
        {
            var result = await _mediator.Send(new DistrictGetByDivisionQuery(divisionId));
            return Ok(result);
        }

        [HttpGet("thanas")]
        public async Task<ActionResult<List<ThanaResponse>>> GetThanas([FromQuery] long districtId)
        {
            var result = await _mediator.Send(new ThanaGetByDistrictQuery(districtId));
            return Ok(result);
        }
    }
}
