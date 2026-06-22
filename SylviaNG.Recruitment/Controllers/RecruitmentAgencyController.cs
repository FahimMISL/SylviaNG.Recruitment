using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyCreate;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyDelete;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyUpdate;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetAll;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Queries.RecruitmentAgencyGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/recruitment-agency")]
    public class RecruitmentAgencyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecruitmentAgencyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RecruitmentAgencyResponse>>> GetAll()
        {
            var result = await _mediator.Send(new RecruitmentAgencyGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RecruitmentAgencyResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RecruitmentAgencyGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{recruitmentAgencyId}")]
        public async Task<ActionResult<RecruitmentAgencyResponse>> GetById(long recruitmentAgencyId)
        {
            var result = await _mediator.Send(new RecruitmentAgencyGetByIdQuery(recruitmentAgencyId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RecruitmentAgencyCreateRequest request)
        {
            var id = await _mediator.Send(new RecruitmentAgencyCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{recruitmentAgencyId}")]
        public async Task<ActionResult> Update(long recruitmentAgencyId, [FromBody] RecruitmentAgencyUpdateRequest request)
        {
            await _mediator.Send(new RecruitmentAgencyUpdateCommand(recruitmentAgencyId, request));
            return Ok();
        }

        [HttpDelete("{recruitmentAgencyId}")]
        public async Task<ActionResult> Delete(long recruitmentAgencyId)
        {
            await _mediator.Send(new RecruitmentAgencyDeleteCommand(recruitmentAgencyId));
            return Ok();
        }
    }
}
