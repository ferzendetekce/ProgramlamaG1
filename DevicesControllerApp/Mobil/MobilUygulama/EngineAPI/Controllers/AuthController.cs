using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RehabilitationSystem.EngineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            //kullaniciGirisDogrula fonk grup 1 e ihtiyac var

            string userRole = null;
            if (request.Username == "grup11" && request.Password == "12345")
                userRole = "Operator";
            if (request.Username == "admin" && request.Password == "admin123")
                userRole = "Admin";
            if (request.Username == "servis" && request.Password == "servis123")
                userRole = "Servis";

            if (userRole != null)
            {
                var token = GenerateJwtToken(request.Username, userRole);
                Console.WriteLine($"Başarılı giriş: {request.Username}, Rol: {userRole}");
                return Ok(new { status = "ok", token });
            }

            Console.WriteLine($"Başarısız giriş denemesi: {request.Username}");
            return Unauthorized(
                new { status = "error", message = "Geçersiz kullanıcı adı veya şifre." }
            );
        }

        private string GenerateJwtToken(string username, string role)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username), // Kullanıcı adı
                new Claim(ClaimTypes.Role, role), // Kullanıcı rolü (Admin, Operator, Servis)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Her token için benzersiz bir ID
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), // Token 8 saat geçerli olacak
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
