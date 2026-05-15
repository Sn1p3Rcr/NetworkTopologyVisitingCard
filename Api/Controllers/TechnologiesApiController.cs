using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Api.Controllers;

[ApiController]
[Route("api/technologies")]
public class TechnologiesController : ControllerBase
{
    private readonly ITechnologyService _technologyService;

    public TechnologiesController(ITechnologyService technologyService)
    {
        _technologyService = technologyService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<TechnologyDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _technologyService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<TechnologyDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var technology = await _technologyService.GetByIdAsync(id, cancellationToken);
        return technology is null ? NotFound() : Ok(technology);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<TechnologyDto>> Create([FromBody] CreateTechnologyDto dto, CancellationToken cancellationToken)
    {
        var technology = await _technologyService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = technology.Id }, technology);
    }

    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<TechnologyDto>> Update(int id, [FromBody] UpdateTechnologyDto dto, CancellationToken cancellationToken)
    {
        var technology = await _technologyService.UpdateAsync(id, dto, cancellationToken);
        return technology is null ? NotFound() : Ok(technology);
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _technologyService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
