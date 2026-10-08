using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using api.Helpers;
using api.Models;
using api.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
namespace api.Services
{
    public static class JWT
    {
        public static string GenerateAccessToken(User User)
        {
            var jwtKey = SafeEnvironment.GetSafeEnvironment("JWT_KEY");
            var jwtIssuer = SafeEnvironment.GetSafeEnvironment("JWT_ISSUER");
            var jwtAudience = SafeEnvironment.GetSafeEnvironment("JWT_AUDIENCE");
            var jwtAM = int.TryParse(SafeEnvironment.GetSafeEnvironment("JWT_AM"),out var m) ? m : 45;
            var expAt = DateTime.UtcNow.AddMinutes(jwtAM);
            var issuedAt = ((long)(DateTime.UtcNow-DateTime.UnixEpoch).TotalSeconds).ToString();
            var Claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,User.Id.ToString()),
                new(JwtRegisteredClaimNames.Email,User.Email.ToString()),
                new(JwtRegisteredClaimNames.Name,User.Name.ToString()),
                new(ClaimTypes.Role,Enum.GetName<RoleEnum>(User.Role)!),
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat,issuedAt)

            };
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken
            (
                issuer:jwtIssuer,
                audience:jwtAudience,
                claims:Claims,
                notBefore:DateTime.UtcNow,
                expires:expAt,
                signingCredentials:creds
            );
            return new JwtSecurityTokenHandler().WriteToken(Token);

        }
        public static (string RefreshToken,DateTime ExpAt) GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }
        public static string HashRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
