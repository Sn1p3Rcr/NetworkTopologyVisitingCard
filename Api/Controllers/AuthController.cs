using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetworkTopologyVisitingCard.DTOs;
using NetworkTopologyVisitingCard.Models;
using NetworkTopologyVisitingCard.Services;

namespace NetworkTopologyVisitingCard.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Unauthorized(new { message = "Неверный email или пароль." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Неверный email или пароль." });

        var token = await _jwtTokenService.GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var expireHours = double.Parse(_configuration["Jwt:ExpireHours"] ?? "24");

        return Ok(new TokenResponseDto(token, DateTime.UtcNow.AddHours(expireHours), user.Email ?? string.Empty, roles.ToList()));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "User");

        var token = await _jwtTokenService.GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var expireHours = double.Parse(_configuration["Jwt:ExpireHours"] ?? "24");

        return Ok(new TokenResponseDto(token, DateTime.UtcNow.AddHours(expireHours), user.Email ?? string.Empty, roles.ToList()));
    }
}
