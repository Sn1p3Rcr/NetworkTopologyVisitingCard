using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.DTOs;

public record NetworkTopologyDto(
    int Id,
    string Name,
    string Description,
    string TopologyData,
    DateTime CreatedDate,
    int ProjectId);

public class CreateNetworkTopologyDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public string TopologyData { get; set; } = "{}";

    public int ProjectId { get; set; }
}

public class UpdateNetworkTopologyDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public string TopologyData { get; set; } = "{}";

    public int ProjectId { get; set; }
}
