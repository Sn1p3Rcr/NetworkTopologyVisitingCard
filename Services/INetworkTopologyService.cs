using NetworkTopologyVisitingCard.DTOs;

namespace NetworkTopologyVisitingCard.Services;

public interface INetworkTopologyService
{
    Task<IReadOnlyList<NetworkTopologyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NetworkTopologyDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<NetworkTopologyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<NetworkTopologyDto?> CreateAsync(CreateNetworkTopologyDto dto, CancellationToken cancellationToken = default);
    Task<NetworkTopologyDto?> UpdateAsync(int id, UpdateNetworkTopologyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
