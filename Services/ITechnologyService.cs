using NetworkTopologyVisitingCard.DTOs;

namespace NetworkTopologyVisitingCard.Services;

public interface ITechnologyService
{
    Task<IReadOnlyList<TechnologyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TechnologyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TechnologyDto> CreateAsync(CreateTechnologyDto dto, CancellationToken cancellationToken = default);
    Task<TechnologyDto?> UpdateAsync(int id, UpdateTechnologyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
