using JwtAuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "u2HkhXmT9ZlPH8M2vFv4hnlM3SfpRtTDfC4H1mn0H20";
        private const int JWT_TOKEN_VALIDITY_MINS = 20;

        public JwtTokenHandler()
        {
        }

        public AuthenticationResponse? GenerateJwtToken(string userName, string role)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(role))
                return null;

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            //var claimsIdentity = new ClaimsIdentity(new List<Claim>
            //{
            //    new Claim(JwtRegisteredClaimNames.Name, authenticationRequest.UserName),
            //    new Claim(ClaimTypes.Role, userAccount.Role)
            //});

            //If used in ocelot configuration file
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim("Role", role)
            });

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                UserName = userName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token,
            };
        }
    }
}
