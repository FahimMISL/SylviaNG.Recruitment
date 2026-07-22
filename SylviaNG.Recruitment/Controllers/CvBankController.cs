using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolAdd;
using SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolRemove;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankSearch;
using SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankTalentPoolGetAll;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Advanced Boolean CV Bank search + a lightweight talent pool bucket (US-045).</summary>
    [ApiController]
    [Route("recruitment/cv-bank")]
    [Authorize(Roles = "Admin,HR")]
    public class CvBankController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CvBankController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Boolean search (AND/OR/NOT, parentheses, quoted phrases) across all candidate profile
        /// fields and CV content, with optional education/experience/location/candidate-type
        /// filters, ranked by relevance (US-045 AC1-AC4).
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<PagedResult<CvBankSearchResultResponse>>> Search([FromBody] CvBankSearchRequest request)
        {
            var result = await _mediator.Send(new CvBankSearchQuery(request));
            return Ok(result);
        }

        /// <summary>Bulk-add search results to the talent pool (US-045 AC5).</summary>
        [HttpPost("talent-pool")]
        public async Task<ActionResult<CvBankTalentPoolAddResponse>> AddToTalentPool([FromBody] CvBankTalentPoolAddRequest request)
        {
            var result = await _mediator.Send(new CvBankTalentPoolAddCommand(request));
            return Ok(result);
        }

        [HttpGet("talent-pool")]
        public async Task<ActionResult<List<CvBankTalentPoolEntryResponse>>> GetTalentPool()
        {
            var result = await _mediator.Send(new CvBankTalentPoolGetAllQuery());
            return Ok(result);
        }

        [HttpDelete("talent-pool/{candidateProfileId:long}")]
        public async Task<ActionResult> RemoveFromTalentPool(long candidateProfileId)
        {
            await _mediator.Send(new CvBankTalentPoolRemoveCommand(candidateProfileId));
            return Ok();
        }
    }
}
