using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Repositories;

namespace NetworkTopologyVisitingCard.Services;

public class NetworkTopologyService : INetworkTopologyService
{
    private readonly IUnitOfWork _unitOfWork;

    public NetworkTopologyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<NetworkTopologyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.NetworkTopologies.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<NetworkTopologyDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.NetworkTopologies.FindAsync(t => t.ProjectId == projectId, cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<NetworkTopologyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.NetworkTopologies.GetByIdAsync(id, cancellationToken);
        return item is null ? null : MapToDto(item);
    }

    public async Task<NetworkTopologyDto?> CreateAsync(CreateNetworkTopologyDto dto, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId, cancellationToken) is null)
            return null;

        var topology = new NetworkTopology
        {
            Name = dto.Name,
            Description = dto.Description,
            TopologyData = dto.TopologyData,
            ProjectId = dto.ProjectId,
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.NetworkTopologies.AddAsync(topology, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(topology);
    }

    public async Task<NetworkTopologyDto?> UpdateAsync(int id, UpdateNetworkTopologyDto dto, CancellationToken cancellationToken = default)
    {
        var topology = await _unitOfWork.NetworkTopologies.GetByIdAsync(id, cancellationToken);
        if (topology is null)
            return null;

        if (await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId, cancellationToken) is null)
            return null;

        topology.Name = dto.Name;
        topology.Description = dto.Description;
        topology.TopologyData = dto.TopologyData;
        topology.ProjectId = dto.ProjectId;

        _unitOfWork.NetworkTopologies.Update(topology);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(topology);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var topology = await _unitOfWork.NetworkTopologies.GetByIdAsync(id, cancellationToken);
        if (topology is null)
            return false;

        _unitOfWork.NetworkTopologies.Remove(topology);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static NetworkTopologyDto MapToDto(NetworkTopology topology) =>
        new(topology.Id, topology.Name, topology.Description, topology.TopologyData,
            topology.CreatedDate, topology.ProjectId);
}
