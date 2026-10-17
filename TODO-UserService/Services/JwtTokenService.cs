using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TODO_UserService.Models;

namespace TODO_UserService.Services;

public class JwtTokenService(IConfiguration configuration)
{
    public string CreateToken(User user)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("displayName", user.DisplayName),
        };

        var expiryMinutes = jwtSection.GetValue<int?>("ExpiryMinutes") ?? 120;

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
