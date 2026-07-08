using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentDelete;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentUpload;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Queries.JobPostingAttachmentGetAllByJobPosting;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/job-posting/{jobPostingId}/attachments")]
    [Authorize(Roles = "Admin,HR")]
    public class JobPostingAttachmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobPostingAttachmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Upload a supporting document for a job posting. Returns attachment metadata only
        /// (the file itself is served separately via static files - see JobPostingAttachmentResponse.DownloadUrl).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<JobPostingAttachmentResponse>> Upload(long jobPostingId, [FromForm] IFormFile file)
        {
            var result = await _mediator.Send(new JobPostingAttachmentUploadCommand(jobPostingId, file));
            return Ok(result);
        }

        /// <summary>
        /// List all attachments for a job posting.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<JobPostingAttachmentResponse>>> GetAll(long jobPostingId)
        {
            var result = await _mediator.Send(new JobPostingAttachmentGetAllByJobPostingQuery(jobPostingId));
            return Ok(result);
        }

        /// <summary>
        /// Delete an attachment from a job posting.
        /// </summary>
        [HttpDelete("{attachmentId}")]
        public async Task<ActionResult> Delete(long jobPostingId, long attachmentId)
        {
            await _mediator.Send(new JobPostingAttachmentDeleteCommand(jobPostingId, attachmentId));
            return Ok();
        }
    }
}
