using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Interfaces;
using MasterNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MasterNet.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    public TokenService(
        IApplicationDbContext context,
        IConfiguration configuration
    ) => (_context, _configuration) = (context, configuration);

    public async Task<string> CreateToken(User user)
    {
        var policies = await _context.Database
            .SqlQuery<string>($@"
                SELECT aspr.""ClaimValue"" AS ""Value""
                FROM ""AspNetUsers"" AS a
                LEFT JOIN ""AspNetUserRoles"" AS ar ON a.""Id"" = ar.""UserId""
                LEFT JOIN ""AspNetRoleClaims"" AS aspr ON ar.""RoleId"" = aspr.""RoleId""
                WHERE a.""Id"" = {user.Id}
            ")
            .ToListAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        foreach (var policy in policies)
        {
            if (policy != null)
            {
                claims.Add(new(CustomClaim.POLICIES, policy));
            }
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}