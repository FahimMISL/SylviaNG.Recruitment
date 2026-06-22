using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationCreate;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationDelete;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/candidate-certification")]
    public class CandidateCertificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateCertificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateCertificationResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CandidateCertificationGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CandidateCertificationResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CandidateCertificationGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{candidateCertificationId}")]
        public async Task<ActionResult<CandidateCertificationResponse>> GetById(long candidateCertificationId)
        {
            var result = await _mediator.Send(new CandidateCertificationGetByIdQuery(candidateCertificationId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CandidateCertificationCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateCertificationCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{candidateCertificationId}")]
        public async Task<ActionResult> Update(long candidateCertificationId, [FromBody] CandidateCertificationUpdateRequest request)
        {
            await _mediator.Send(new CandidateCertificationUpdateCommand(candidateCertificationId, request));
            return Ok();
        }

        [HttpDelete("{candidateCertificationId}")]
        public async Task<ActionResult> Delete(long candidateCertificationId)
        {
            await _mediator.Send(new CandidateCertificationDeleteCommand(candidateCertificationId));
            return Ok();
        }
    }
}
