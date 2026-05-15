using NetworkTopologyVisitingCard.DTOs;

namespace NetworkTopologyVisitingCard.Services;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReviewDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ReviewDto?> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken = default);
    Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
