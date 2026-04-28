using GymManagement.Application.DTOs;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers;

/// <summary>
/// Handles HTTP operations for workout sessions.
/// </summary>
[ApiController]
[Route("api/workout-sessions")]
[Produces("application/json")]
public class WorkoutSessionsController : ControllerBase
{
    private readonly IWorkoutSessionService _workoutSessionService;

    public WorkoutSessionsController(IWorkoutSessionService workoutSessionService)
    {
        _workoutSessionService = workoutSessionService;
    }

    /// <summary>
    /// Retrieves a workout session by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkoutSessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var session = await _workoutSessionService.GetByIdAsync(id, cancellationToken);
        return session is null ? NotFound(new { message = $"Session {id} not found." }) : Ok(session);
    }

    /// <summary>
    /// Registers a new workout session.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkoutSessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWorkoutSessionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var session = await _workoutSessionService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
