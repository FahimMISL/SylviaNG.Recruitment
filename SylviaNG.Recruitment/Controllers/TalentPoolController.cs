using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCandidateAdd;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCandidateRemove;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCreate;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolDelete;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolFastTrack;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAll;
using SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetById;
using SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolLookup;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Named talent pools HR can revisit for future vacancies (US-039).</summary>
    [ApiController]
    [Route("recruitment/talent-pool")]
    [Authorize(Roles = "Admin,HR")]
    public class TalentPoolController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TalentPoolController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>All pools with candidate counts (AC1 pool list page).</summary>
        [HttpGet]
        public async Task<ActionResult<List<TalentPoolResponse>>> GetAll()
        {
            var result = await _mediator.Send(new TalentPoolGetAllQuery());
            return Ok(result);
        }

        /// <summary>Id+Name only, for the "add to pool" picker dropdown.</summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<TalentPoolLookupResponse>>> GetLookup()
        {
            var result = await _mediator.Send(new TalentPoolLookupQuery());
            return Ok(result);
        }

        /// <summary>Pool detail: every member with their latest profile snapshot (AC3).</summary>
        [HttpGet("{talentPoolId:long}")]
        public async Task<ActionResult<TalentPoolDetailResponse>> GetById(long talentPoolId)
        {
            var result = await _mediator.Send(new TalentPoolGetByIdQuery(talentPoolId));
            return Ok(result);
        }

        /// <summary>Create a new named talent pool (AC1).</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TalentPoolCreateRequest request)
        {
            var id = await _mediator.Send(new TalentPoolCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Delete a pool and its memberships (candidates themselves are untouched).</summary>
        [HttpDelete("{talentPoolId:long}")]
        public async Task<ActionResult> Delete(long talentPoolId)
        {
            await _mediator.Send(new TalentPoolDeleteCommand(talentPoolId));
            return Ok();
        }

        /// <summary>Bulk-add candidates to a pool from any point in the pipeline (AC1).</summary>
        [HttpPost("{talentPoolId:long}/candidates")]
        public async Task<ActionResult<TalentPoolCandidateAddResponse>> AddCandidates(long talentPoolId, [FromBody] TalentPoolCandidateAddRequest request)
        {
            var result = await _mediator.Send(new TalentPoolCandidateAddCommand(talentPoolId, request));
            return Ok(result);
        }

        /// <summary>Remove a candidate from a pool at any time (AC5).</summary>
        [HttpDelete("{talentPoolId:long}/candidates/{candidateProfileId:long}")]
        public async Task<ActionResult> RemoveCandidate(long talentPoolId, long candidateProfileId)
        {
            await _mediator.Send(new TalentPoolCandidateRemoveCommand(talentPoolId, candidateProfileId));
            return Ok();
        }

        /// <summary>Fast-track pool candidates straight to Shortlisted on a newly-opened vacancy (AC4).</summary>
        [HttpPost("fast-track")]
        public async Task<ActionResult<TalentPoolFastTrackResponse>> FastTrack([FromBody] TalentPoolFastTrackRequest request)
        {
            var result = await _mediator.Send(new TalentPoolFastTrackCommand(request));
            return Ok(result);
        }
    }
}
