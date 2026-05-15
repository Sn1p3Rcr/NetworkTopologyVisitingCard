using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.DTOs;

public record ProjectDto(
    int Id,
    string Title,
    string Description,
    DateTime CreatedDate,
    string Technologies,
    string ImageUrl,
    string Author);

public class CreateProjectDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Technologies { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;
}

public class UpdateProjectDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Technologies { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;
}
