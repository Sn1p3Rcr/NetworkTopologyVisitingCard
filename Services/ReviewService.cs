using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Repositories;

namespace NetworkTopologyVisitingCard.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ReviewDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetAllAsync(cancellationToken);
        return reviews.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<ReviewDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => r.ProjectId == projectId, cancellationToken);
        return reviews.Select(MapToDto).ToList();
    }

    public async Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
        return review is null ? null : MapToDto(review);
    }

    public async Task<ReviewDto?> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId, cancellationToken) is null)
            return null;

        var review = new Review
        {
            AuthorName = dto.AuthorName,
            Content = dto.Content,
            Rating = dto.Rating,
            ProjectId = dto.ProjectId,
            ReviewDate = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(review);
    }

    public async Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null)
            return null;

        if (await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId, cancellationToken) is null)
            return null;

        review.AuthorName = dto.AuthorName;
        review.Content = dto.Content;
        review.Rating = dto.Rating;
        review.ProjectId = dto.ProjectId;

        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(review);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null)
            return false;

        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ReviewDto MapToDto(Review review) =>
        new(review.Id, review.AuthorName, review.Content, review.ReviewDate, review.Rating, review.ProjectId);
}
