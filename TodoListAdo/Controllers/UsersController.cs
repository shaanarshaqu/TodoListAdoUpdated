using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoListAdo.Depandancies;
using TodoListAdo.Models;

namespace TodoListAdo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsers _usersData;
        private readonly IConfiguration _configuration;
        public UsersController(IUsers usersData, IConfiguration configuration) 
        {
            _usersData=usersData;
            _configuration=configuration;
        }






        [HttpGet(Name = "GetData")]
        [Authorize(Roles ="admin")]
        public IActionResult Get()
        {
            int a = 0;
            return Ok(_usersData.DisplayUsers());
        }



        [HttpPost("validate")]
        public IActionResult ValidateUser([FromBody] inputUserDTO user)
        {
            if (user == null)
            {
                return BadRequest("Enter validcredentials");
            }
            var userIsExist = _usersData.DisplayUsers().FirstOrDefault(x=>x.UserName == user.UserName && x.Password == user.Password);
            if (userIsExist == null)
            {
                return NotFound(user.UserName);
            }
            var userdetail = _usersData.Login(user);

            string token = GenerateJwtToken(userdetail);

            return Ok(new { Token = token , Id = userdetail .Id});

        }
        private string GenerateJwtToken(UsersDTO user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            // Add additional claims as needed
        };

            var token = new JwtSecurityToken(
                //issuer: _configuration["Jwt:Issuer"],
                //audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
