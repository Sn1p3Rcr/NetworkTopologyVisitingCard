using Moq;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Repositories;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Review>> _reviewRepositoryMock;
    private readonly Mock<IRepository<Project>> _projectRepositoryMock;
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _reviewRepositoryMock = new Mock<IRepository<Review>>();
        _projectRepositoryMock = new Mock<IRepository<Project>>();
        _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);
        _service = new ReviewService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectNotFound_ReturnsNull()
    {
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

        var result = await _service.CreateAsync(new CreateReviewDto
        {
            AuthorName = "User",
            Content = "Great",
            Rating = 5,
            ProjectId = 1
        });

        Assert.Null(result);
        _reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectExists_CreatesReview()
    {
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Project { Id = 1, Title = "P", Description = "D", Author = "A" });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.CreateAsync(new CreateReviewDto
        {
            AuthorName = "User",
            Content = "Great",
            Rating = 5,
            ProjectId = 1
        });

        Assert.NotNull(result);
        Assert.Equal(5, result.Rating);
    }
}
