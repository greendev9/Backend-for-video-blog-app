using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models.Auth
{
   
    public class AuthToken 
    { 
        private readonly JwtSecurityToken _token;  
        public AuthToken(string email, string issuer, string jwtKey, int id)
        { 
            var claims = new[] { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.NameIdentifier,  $"{id}") };  
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            _token = new JwtSecurityToken(
                 issuer: issuer,
                 audience: issuer,
                 claims: claims,
                 expires: DateTime.Now.AddMinutes(930),
                 signingCredentials: creds); 
        } 
        

        public override string ToString() =>
            new JwtSecurityTokenHandler().WriteToken(_token); 
         
    }
}
