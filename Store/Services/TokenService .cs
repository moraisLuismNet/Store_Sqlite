using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Store.DTOs;

namespace Store.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private LoginResponseDTO GenerateToken(UserDTO credentialsUser)
        {
            var claims = new List<Claim>()
         {
             new Claim(ClaimTypes.Email, credentialsUser.Email),
             new Claim("whatever i want", "any other value")
         };

            var key = _configuration["JWTKey"];
            var keyKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signinCredentials = new SigningCredentials(keyKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: signinCredentials
            );


            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new LoginResponseDTO()
            {
                Token = tokenString,
                Email = credentialsUser.Email
            };
        }
    }
}
