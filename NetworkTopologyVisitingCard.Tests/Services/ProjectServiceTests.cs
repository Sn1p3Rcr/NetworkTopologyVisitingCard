using Moq;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Repositories;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Project>> _projectRepositoryMock;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _projectRepositoryMock = new Mock<IRepository<Project>>();
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);
        _service = new ProjectService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProjects()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, Title = "A", Description = "Desc", CreatedDate = DateTime.UtcNow, Author = "Author" }
        };
        _projectRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(projects);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("A", result[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsProjectAndSaves()
    {
        var dto = new CreateProjectDto
        {
            Title = "New",
            Description = "Description",
            Author = "Tester"
        };

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.CreateAsync(dto, "user-1");

        Assert.Equal("New", result.Title);
        _projectRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ReturnsTrue()
    {
        var project = new Project { Id = 1, Title = "X", Description = "D", Author = "A" };
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _projectRepositoryMock.Verify(r => r.Remove(project), Times.Once);
    }

    [Fact]
    public async Task CanEditProjectAsync_WhenOwner_ReturnsTrue()
    {
        var project = new Project { Id = 1, Title = "X", Description = "D", Author = "A", OwnerId = "user-1" };
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var result = await _service.CanEditProjectAsync(1, "user-1", false);

        Assert.True(result);
    }

    [Fact]
    public async Task CanEditProjectAsync_WhenDifferentUser_ReturnsFalse()
    {
        var project = new Project { Id = 1, Title = "X", Description = "D", Author = "A", OwnerId = "user-1" };
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var result = await _service.CanEditProjectAsync(1, "user-2", false);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ReturnsFalse()
    {
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
    }
}
