using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IdentityExploration.Controllers.Auths
{
    public interface Token
    {
        public string GetToken(List<Claim> claims);
    }
    public class JWT : Token
    {
        private string _issuer;
        private string _audience;
        private string _secretKey;
        private int _expirationInMinutes;

        public JWT(IConfiguration configuration)
        {
            _issuer = configuration.GetValue<string>("JwtSettings:Issuer");
            _audience = configuration.GetValue<string>("JwtSettings:Audience");
            _secretKey = configuration.GetValue<string>("JwtSettings:SecretKey");
            _expirationInMinutes = configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");
        }

        public string GetToken(List<Claim> claims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var claimsIdentity = new ClaimsIdentity(claims);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_expirationInMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
