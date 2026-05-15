using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsApiController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsApiController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _projectService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(id, cancellationToken);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin")]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto, CancellationToken cancellationToken)
    {
        var project = await _projectService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationToken)
    {
        var project = await _projectService.UpdateAsync(id, dto, cancellationToken);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _projectService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
