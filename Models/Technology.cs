using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.Models;

public class Technology
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    public ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = new List<ProjectTechnology>();
}
