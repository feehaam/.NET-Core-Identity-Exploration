using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IdentityExploration.Controllers.Auths
{
    // 14. Defined this interface for adding Token as a service rather than static class, I hate static class
    public interface Token
    {
        public string GetToken(List<Claim> claims);
    }
    public class JWT : Token
    {

        // 15. This class reads all necessary values from the appsettings.json then uses those 
        // to create secured hash code using SHA-256. The time limit for session is also defined in appsettings
        // 16. These setup are for generating the hash code however when an user logins then the decryption and
        // validation of the hashed JWT token are done by .net core itself with the values defined in program.cs
        // file. Though all values are approach are same only difference is this is encryption and that is
        // decryption See comment #17 in program.cs.
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
