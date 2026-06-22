using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestCreate;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestDelete;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestUpdate;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetAll;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/medical-test")]
    public class MedicalTestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicalTestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicalTestResponse>>> GetAll()
        {
            var result = await _mediator.Send(new MedicalTestGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<MedicalTestResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new MedicalTestGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{medicalTestId}")]
        public async Task<ActionResult<MedicalTestResponse>> GetById(long medicalTestId)
        {
            var result = await _mediator.Send(new MedicalTestGetByIdQuery(medicalTestId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] MedicalTestCreateRequest request)
        {
            var id = await _mediator.Send(new MedicalTestCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{medicalTestId}")]
        public async Task<ActionResult> Update(long medicalTestId, [FromBody] MedicalTestUpdateRequest request)
        {
            await _mediator.Send(new MedicalTestUpdateCommand(medicalTestId, request));
            return Ok();
        }

        [HttpDelete("{medicalTestId}")]
        public async Task<ActionResult> Delete(long medicalTestId)
        {
            await _mediator.Send(new MedicalTestDeleteCommand(medicalTestId));
            return Ok();
        }
    }
}
