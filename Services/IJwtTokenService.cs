using NetworkTopologyVisitingCard.Models;

namespace NetworkTopologyVisitingCard.Services;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}
