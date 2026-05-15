using Microsoft.AspNetCore.Identity;

namespace NetworkTopologyVisitingCard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}