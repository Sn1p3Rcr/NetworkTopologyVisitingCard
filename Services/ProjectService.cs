using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Repositories;

namespace NetworkTopologyVisitingCard.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var projects = await _unitOfWork.Projects.GetAllAsync(cancellationToken);
        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
        return project is null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, string ownerId, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            CreatedDate = DateTime.UtcNow,
            Technologies = dto.Technologies,
            ImageUrl = dto.ImageUrl,
            Author = dto.Author,
            OwnerId = ownerId
        };

        await _unitOfWork.Projects.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(project);
    }

    public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
        if (project is null)
            return null;

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.Technologies = dto.Technologies;
        project.ImageUrl = dto.ImageUrl;
        project.Author = dto.Author;

        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(project);
    }

    public async Task<bool> CanEditProjectAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        if (isAdmin)
            return true;

        if (string.IsNullOrEmpty(userId))
            return false;

        var project = await _unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
        return project is not null && project.OwnerId == userId;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
        if (project is null)
            return false;

        _unitOfWork.Projects.Remove(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProjectDto MapToDto(Project project) =>
        new(project.Id, project.Title, project.Description, project.CreatedDate,
            project.Technologies, project.ImageUrl, project.Author);
}
