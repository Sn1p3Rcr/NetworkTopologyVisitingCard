using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _reviewService.GetAllAsync(cancellationToken));
    }

    [HttpGet("by-project/{projectId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetByProject(int projectId, CancellationToken cancellationToken)
    {
        return Ok(await _reviewService.GetByProjectIdAsync(projectId, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ReviewDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await _reviewService.GetByIdAsync(id, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin")]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var review = await _reviewService.CreateAsync(dto, cancellationToken);
        return review is null ? BadRequest(new { message = "Проект не найден." }) : CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin")]
    public async Task<ActionResult<ReviewDto>> Update(int id, [FromBody] UpdateReviewDto dto, CancellationToken cancellationToken)
    {
        var review = await _reviewService.UpdateAsync(id, dto, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _reviewService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
