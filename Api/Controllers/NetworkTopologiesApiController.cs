using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Api.Controllers;

[ApiController]
[Route("api/networktopologies")]
public class NetworkTopologiesController : ControllerBase
{
    private readonly INetworkTopologyService _topologyService;

    public NetworkTopologiesController(INetworkTopologyService topologyService)
    {
        _topologyService = topologyService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<NetworkTopologyDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _topologyService.GetAllAsync(cancellationToken));
    }

    [HttpGet("by-project/{projectId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<NetworkTopologyDto>>> GetByProject(int projectId, CancellationToken cancellationToken)
    {
        return Ok(await _topologyService.GetByProjectIdAsync(projectId, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<NetworkTopologyDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var topology = await _topologyService.GetByIdAsync(id, cancellationToken);
        return topology is null ? NotFound() : Ok(topology);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin")]
    public async Task<ActionResult<NetworkTopologyDto>> Create([FromBody] CreateNetworkTopologyDto dto, CancellationToken cancellationToken)
    {
        var topology = await _topologyService.CreateAsync(dto, cancellationToken);
        return topology is null
            ? BadRequest(new { message = "Проект не найден." })
            : CreatedAtAction(nameof(GetById), new { id = topology.Id }, topology);
    }

    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin")]
    public async Task<ActionResult<NetworkTopologyDto>> Update(int id, [FromBody] UpdateNetworkTopologyDto dto, CancellationToken cancellationToken)
    {
        var topology = await _topologyService.UpdateAsync(id, dto, cancellationToken);
        return topology is null ? NotFound() : Ok(topology);
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _topologyService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
