using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MiniERP.Application.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // appsettings.json dosyasındaki o gizli şifreleri okumak için IConfiguration kullanıyoruz
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            // SENIOR NOTU: Şimdilik sistemi test etmek için kullanıcı adı ve şifreyi sabit (admin/123456) veriyoruz.
            // Gerçek ve büyük bir projede burada veritabanına gidip "Böyle bir kullanıcı var mı ve şifresi doğru mu?" diye bakarız.

            if (loginDto.Username == "admin" && loginDto.Password == "123456")
            {
                // 1. appsettings.json'dan gizli ayarlarımızı çekiyoruz
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                // 2. Token'ın içine koyacağımız kimlik bilgileri (Kullanıcının adı ve benzersiz bir ID)
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, loginDto.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // 3. Token'ın kurallarını belirliyoruz (30 dakika geçerli olacak ve bizim gizli anahtarımızla şifrelenecek)
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                // 4. Token'ı üretiyoruz ve kullanıcıya gönderiyoruz
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var stringToken = tokenHandler.WriteToken(token);

                return Ok(new { token = stringToken }); // Başarılı! Token'ı JSON olarak dön.
            }

            // Eğer şifre veya kullanıcı adı yanlışsa 401 (İzinsiz Giriş) hatası dönüyoruz.
            return Unauthorized("Kullanıcı adı veya şifre hatalı!");
        }
    }
}