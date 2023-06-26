using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IdentityExploration.Controllers
{
    public static class JwtHelper
    {
        public static string GenerateToken(string secretKey, string issuer, string audience, int expirationInMinutes, ClaimsIdentity claimsIdentity)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateToken(string secretKey, string issuer, string audience, int expirationInMinutes, IEnumerable<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims);
            return GenerateToken(secretKey, issuer, audience, expirationInMinutes, claimsIdentity);
        }
    }

}
