using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string Password { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
}

public record TokenResponseDto(string Token, DateTime Expires, string Email, IList<string> Roles);
