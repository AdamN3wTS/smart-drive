using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using api.Helpers;
using api.Models;
using api.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.WebUtilities;
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
        public static (string RefreshTokenPlain,string RefreshTokenHash,DateTime ExpAt) GenerateRefreshToken()
        {
            var jwtAD = int.TryParse(SafeEnvironment.GetSafeEnvironment("JWT_AD"),out var m) ? m : 45;
            var expAt = DateTime.UtcNow.AddDays(jwtAD);
            var bytes = RandomNumberGenerator.GetBytes(32);
            var plain = WebEncoders.Base64UrlEncode(bytes);
            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(plain)));
            return (plain,hash,expAt);
            
        }
        
    }
}
