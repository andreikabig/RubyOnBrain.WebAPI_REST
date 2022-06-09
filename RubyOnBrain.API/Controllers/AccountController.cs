using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RubyOnBrain.API.Models;
using RubyOnBrain.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RubyOnBrain.API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly DataContext db;

        public AccountController(DataContext db)
        {
            this.db = db;
        }

        // POST: api/auth
        [HttpPost("api/auth")]
        public IActionResult Token([FromBody] UserAuthDTO user)
        {
            if (user == null)
                return BadRequest();

            var identity = GetIdentity(user.Username, user.Password);

            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password" });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new {
                access_token = encodedJwt,
                username = identity.Name,
                role = identity.Claims.Last().Value.ToString()
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = db.Users.Include(x => x.Role).FirstOrDefault(x => x.Email == username && x.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }
    }
}
