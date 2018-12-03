using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using authen.Model;

//https://developer.okta.com/blog/2018/03/23/token-authentication-aspnetcore-complete-guide
namespace authen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration config)
        {
            _configuration = config;
        }
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Only if you got valid token";
        }
        [AllowAnonymous]
        [HttpPost("RequestToken")]
        [Route("token")]
        public IActionResult RequestToken(User user)
        {
            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {
               var claims = new[]
               {
                   new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                   new Claim(JwtRegisteredClaimNames.Sub, user.Email)
               };
               var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityToken:Key"])); 
               var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

               var jwtSecurityToken = new JwtSecurityToken(
                   issuer: _configuration["JwtSecurityToken:Issuer"],
                   audience: _configuration["JwtSecurityToken:Audience"],
                   claims: claims,
                   expires: DateTime.UtcNow.AddMinutes(10),
                   signingCredentials: signingCredentials
               );
               return Ok(new
               {
                   token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                   expiration = jwtSecurityToken.ValidTo
               });
               
            }
            return Unauthorized();
        }
    }
}