using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SistemaPedidos.Api.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(string usuario, string rol)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario),
            new(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),
            new("role", rol)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(
                    _configuration["Jwt:ExpirationMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}