using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPedidos.Api.Services;

namespace SistemaPedidos.Api.Controllers;

public record LoginRequest(
    string Usuario,
    string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtService;
    private readonly IWebHostEnvironment _environment;

    public AuthController(
        JwtTokenService jwtService,
        IWebHostEnvironment environment)
    {
        _jwtService = jwtService;
        _environment = environment;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Usuarios ficticios, únicamente para desarrollo.
        if (!_environment.IsDevelopment())
            return NotFound();

        string? rol = null;

        if (request.Usuario == "admin" &&
            request.Password == "Admin123!")
        {
            rol = "Administrador";
        }
        else if (request.Usuario == "usuario" &&
                 request.Password == "Usuario123!")
        {
            rol = "Usuario";
        }

        if (rol is null)
            return Unauthorized(new
            {
                Mensaje = "Credenciales incorrectas."
            });

        var token = _jwtService.GenerarToken(
            request.Usuario, rol);

        return Ok(new
        {
            AccessToken = token,
            TokenType = "Bearer",
            Rol = rol
        });
    }
}