using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Repositories;

namespace NetworkTopologyVisitingCard.Services;

public class TechnologyService : ITechnologyService
{
    private readonly IUnitOfWork _unitOfWork;

    public TechnologyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TechnologyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Technologies.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<TechnologyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Technologies.GetByIdAsync(id, cancellationToken);
        return item is null ? null : MapToDto(item);
    }

    public async Task<TechnologyDto> CreateAsync(CreateTechnologyDto dto, CancellationToken cancellationToken = default)
    {
        var technology = new Technology
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category
        };

        await _unitOfWork.Technologies.AddAsync(technology, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(technology);
    }

    public async Task<TechnologyDto?> UpdateAsync(int id, UpdateTechnologyDto dto, CancellationToken cancellationToken = default)
    {
        var technology = await _unitOfWork.Technologies.GetByIdAsync(id, cancellationToken);
        if (technology is null)
            return null;

        technology.Name = dto.Name;
        technology.Description = dto.Description;
        technology.Category = dto.Category;

        _unitOfWork.Technologies.Update(technology);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(technology);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var technology = await _unitOfWork.Technologies.GetByIdAsync(id, cancellationToken);
        if (technology is null)
            return false;

        _unitOfWork.Technologies.Remove(technology);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TechnologyDto MapToDto(Technology technology) =>
        new(technology.Id, technology.Name, technology.Description, technology.Category);
}
