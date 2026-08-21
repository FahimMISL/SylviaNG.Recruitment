using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.DocumentTracking.Commands.DocumentTrackingFollowUp;
using SylviaNG.Recruitment.Application.Features.DocumentTracking.Models;
using SylviaNG.Recruitment.Application.Features.DocumentTracking.Queries.DocumentTrackingGetAll;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-085: HR-facing combined acceptance-status view over OfferLetters + AppointmentLetters.
    [ApiController]
    [Route("recruitment/document-tracking")]
    [Authorize(Roles = "Admin,HR")]
    public class DocumentTrackingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentTrackingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<DocumentTrackingItemResponse>>> GetAll([FromQuery] DocumentTrackingFilterRequest filter)
        {
            return Ok(await _mediator.Send(new DocumentTrackingGetAllQuery(filter)));
        }

        [HttpPost("{documentType}/{sourceId}/follow-up")]
        public async Task<ActionResult> FollowUp(DocumentTypeEnum documentType, long sourceId)
        {
            await _mediator.Send(new DocumentTrackingFollowUpCommand(documentType, sourceId));
            return NoContent();
        }
    }
}
