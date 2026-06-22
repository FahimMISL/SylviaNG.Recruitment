using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentCreate;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentDelete;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Queries.CandidateDocumentGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/candidate-document")]
    public class CandidateDocumentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateDocumentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateDocumentResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CandidateDocumentGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CandidateDocumentResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CandidateDocumentGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{candidateDocumentId}")]
        public async Task<ActionResult<CandidateDocumentResponse>> GetById(long candidateDocumentId)
        {
            var result = await _mediator.Send(new CandidateDocumentGetByIdQuery(candidateDocumentId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CandidateDocumentCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateDocumentCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{candidateDocumentId}")]
        public async Task<ActionResult> Update(long candidateDocumentId, [FromBody] CandidateDocumentUpdateRequest request)
        {
            await _mediator.Send(new CandidateDocumentUpdateCommand(candidateDocumentId, request));
            return Ok();
        }

        [HttpDelete("{candidateDocumentId}")]
        public async Task<ActionResult> Delete(long candidateDocumentId)
        {
            await _mediator.Send(new CandidateDocumentDeleteCommand(candidateDocumentId));
            return Ok();
        }
    }
}
