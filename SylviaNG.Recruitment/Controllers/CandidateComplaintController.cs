using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintCreate;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintDelete;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/candidate-complaint")]
    public class CandidateComplaintController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateComplaintController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateComplaintResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CandidateComplaintGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CandidateComplaintResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CandidateComplaintGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{candidateComplaintId}")]
        public async Task<ActionResult<CandidateComplaintResponse>> GetById(long candidateComplaintId)
        {
            var result = await _mediator.Send(new CandidateComplaintGetByIdQuery(candidateComplaintId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CandidateComplaintCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateComplaintCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{candidateComplaintId}")]
        public async Task<ActionResult> Update(long candidateComplaintId, [FromBody] CandidateComplaintUpdateRequest request)
        {
            await _mediator.Send(new CandidateComplaintUpdateCommand(candidateComplaintId, request));
            return Ok();
        }

        [HttpDelete("{candidateComplaintId}")]
        public async Task<ActionResult> Delete(long candidateComplaintId)
        {
            await _mediator.Send(new CandidateComplaintDeleteCommand(candidateComplaintId));
            return Ok();
        }
    }
}
