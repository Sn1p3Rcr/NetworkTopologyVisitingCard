using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.DTOs;

public record ReviewDto(
    int Id,
    string AuthorName,
    string Content,
    DateTime ReviewDate,
    int Rating,
    int ProjectId);

public class CreateReviewDto
{
    [Required]
    public string AuthorName { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    public int ProjectId { get; set; }
}

public class UpdateReviewDto
{
    [Required]
    public string AuthorName { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    public int ProjectId { get; set; }
}
