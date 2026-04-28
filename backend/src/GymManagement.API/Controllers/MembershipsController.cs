using GymManagement.Application.DTOs;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers;

/// <summary>
/// Handles HTTP operations for memberships.
/// </summary>
[ApiController]
[Route("api/memberships")]
[Produces("application/json")]
public class MembershipsController : ControllerBase
{
    private readonly IMembershipService _membershipService;

    public MembershipsController(IMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    /// <summary>
    /// Retrieves a membership by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var membership = await _membershipService.GetByIdAsync(id, cancellationToken);
        return membership is null ? NotFound(new { message = $"Membership {id} not found." }) : Ok(membership);
    }

    /// <summary>
    /// Registers a new membership for a client.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMembershipDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var membership = await _membershipService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = membership.Id }, membership);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Renews an existing membership.
    /// </summary>
    [HttpPost("{id:int}/renew")]
    [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Renew(int id, [FromBody] RenewMembershipDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var membership = await _membershipService.RenewAsync(id, dto, cancellationToken);
            return membership is null ? NotFound(new { message = $"Membership {id} not found." }) : Ok(membership);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Suspends a membership.
    /// </summary>
    [HttpPost("{id:int}/suspend")]
    [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Suspend(int id, CancellationToken cancellationToken)
    {
        var membership = await _membershipService.SuspendAsync(id, cancellationToken);
        return membership is null ? NotFound(new { message = $"Membership {id} not found." }) : Ok(membership);
    }

    /// <summary>
    /// Cancels a membership.
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var membership = await _membershipService.CancelAsync(id, cancellationToken);
        return membership is null ? NotFound(new { message = $"Membership {id} not found." }) : Ok(membership);
    }
}
