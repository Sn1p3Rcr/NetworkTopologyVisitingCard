using Moq;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Repositories;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Tests.Services;

public class TechnologyServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Technology>> _technologyRepositoryMock;
    private readonly TechnologyService _service;

    public TechnologyServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _technologyRepositoryMock = new Mock<IRepository<Technology>>();
        _unitOfWorkMock.Setup(u => u.Technologies).Returns(_technologyRepositoryMock.Object);
        _service = new TechnologyService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ReturnsNull()
    {
        _technologyRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Technology?)null);

        var result = await _service.UpdateAsync(1, new UpdateTechnologyDto { Name = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesTechnology()
    {
        var technology = new Technology { Id = 1, Name = "Old", Category = "Cat" };
        _technologyRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(technology);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.UpdateAsync(1, new UpdateTechnologyDto
        {
            Name = "New",
            Description = "Desc",
            Category = "Backend"
        });

        Assert.NotNull(result);
        Assert.Equal("New", result.Name);
    }
}
