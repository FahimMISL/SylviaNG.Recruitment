using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailCreate;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailDelete;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailUpdate;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetAll;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/insurance-detail")]
    public class InsuranceDetailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InsuranceDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InsuranceDetailResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InsuranceDetailGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InsuranceDetailResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InsuranceDetailGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{insuranceDetailId}")]
        public async Task<ActionResult<InsuranceDetailResponse>> GetById(long insuranceDetailId)
        {
            var result = await _mediator.Send(new InsuranceDetailGetByIdQuery(insuranceDetailId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InsuranceDetailCreateRequest request)
        {
            var id = await _mediator.Send(new InsuranceDetailCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{insuranceDetailId}")]
        public async Task<ActionResult> Update(long insuranceDetailId, [FromBody] InsuranceDetailUpdateRequest request)
        {
            await _mediator.Send(new InsuranceDetailUpdateCommand(insuranceDetailId, request));
            return Ok();
        }

        [HttpDelete("{insuranceDetailId}")]
        public async Task<ActionResult> Delete(long insuranceDetailId)
        {
            await _mediator.Send(new InsuranceDetailDeleteCommand(insuranceDetailId));
            return Ok();
        }
    }
}
