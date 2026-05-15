using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.DTOs;

public record TechnologyDto(
    int Id,
    string Name,
    string Description,
    string Category);

public class CreateTechnologyDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
}

public class UpdateTechnologyDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
}
