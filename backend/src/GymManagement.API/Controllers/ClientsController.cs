using GymManagement.Application.DTOs;
using GymManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers;

/// <summary>
/// Handles HTTP operations for gym clients.
/// (SRP - Only routes HTTP requests, delegates to IClientService)
/// (DIP - Depends on IClientService abstraction)
/// </summary>
[ApiController]
[Route("api/clients")]
[Produces("application/json")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all clients.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var clients = await _clientService.GetAllAsync(cancellationToken);
        return Ok(clients);
    }

    /// <summary>
    /// Retrieves a specific client by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetByIdAsync(id, cancellationToken);
        return client is null ? NotFound(new { message = $"Client {id} not found." }) : Ok(client);
    }

    /// <summary>
    /// Registers a new client.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateClientDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _clientService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates a client's information.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientDto dto, CancellationToken cancellationToken)
    {
        var client = await _clientService.UpdateAsync(id, dto, cancellationToken);
        return client is null ? NotFound(new { message = $"Client {id} not found." }) : Ok(client);
    }

    /// <summary>
    /// Gets all memberships for a client.
    /// </summary>
    [HttpGet("{id:int}/memberships")]
    [ProducesResponseType(typeof(IEnumerable<MembershipDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMemberships(int id,
        [FromServices] IMembershipService membershipService,
        CancellationToken cancellationToken)
    {
        var memberships = await membershipService.GetByClientIdAsync(id, cancellationToken);
        return Ok(memberships);
    }

    /// <summary>
    /// Gets all workout sessions for a client.
    /// </summary>
    [HttpGet("{id:int}/workout-sessions")]
    [ProducesResponseType(typeof(IEnumerable<WorkoutSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkoutSessions(int id,
        [FromServices] IWorkoutSessionService workoutSessionService,
        CancellationToken cancellationToken)
    {
        var sessions = await workoutSessionService.GetByClientIdAsync(id, cancellationToken);
        return Ok(sessions);
    }
}
