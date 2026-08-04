using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletBulkGenerate;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletGenerate;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletBulkDownload;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetAll;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetById;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetEligibleCandidates;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-084: HR-only, mirrors AppointmentLetterController's shape + CvBankController's
    // bulk-download-as-File-result pattern.
    [ApiController]
    [Route("recruitment/joining-booklet")]
    [Authorize(Roles = "Admin,HR")]
    public class JoiningBookletController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JoiningBookletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("eligible-candidates")]
        public async Task<ActionResult<List<JoiningBookletEligibleCandidateResponse>>> GetEligibleCandidates()
        {
            return Ok(await _mediator.Send(new JoiningBookletGetEligibleCandidatesQuery()));
        }

        [HttpGet]
        public async Task<ActionResult<List<JoiningBookletResponse>>> GetAll([FromQuery] long? jobApplicationId)
        {
            return Ok(await _mediator.Send(new JoiningBookletGetAllQuery(jobApplicationId)));
        }

        [HttpGet("{joiningBookletId}")]
        public async Task<ActionResult<JoiningBookletResponse>> GetById(long joiningBookletId)
        {
            return Ok(await _mediator.Send(new JoiningBookletGetByIdQuery(joiningBookletId)));
        }

        [HttpPost("generate")]
        public async Task<ActionResult<JoiningBookletResponse>> Generate([FromBody] JoiningBookletGenerateRequest request)
        {
            return Ok(await _mediator.Send(new JoiningBookletGenerateCommand(request)));
        }

        [HttpPost("bulk-generate")]
        public async Task<ActionResult<JoiningBookletBulkGenerateResponse>> BulkGenerate([FromBody] JoiningBookletBulkGenerateRequest request)
        {
            return Ok(await _mediator.Send(new JoiningBookletBulkGenerateCommand(request)));
        }

        [HttpPost("bulk-download")]
        public async Task<IActionResult> BulkDownload([FromBody] JoiningBookletBulkDownloadRequest request)
        {
            var file = await _mediator.Send(new JoiningBookletBulkDownloadQuery(request));
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
