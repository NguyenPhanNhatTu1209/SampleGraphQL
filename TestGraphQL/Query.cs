
using HotChocolate.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TestGraphQL
{
    public class Query
    {
        public string getToken([Service]IOptions<TokenSettings> tokenSettings)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: tokenSettings.Value.Issure,
                audience: tokenSettings.Value.Audience,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        [Authorize]
        public string CheckAuthorize()
        {
            return "Success";   
        }
    }
}
