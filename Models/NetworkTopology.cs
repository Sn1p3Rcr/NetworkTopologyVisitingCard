using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.Models;

public class NetworkTopology
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public string TopologyData { get; set; } = "{}";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
