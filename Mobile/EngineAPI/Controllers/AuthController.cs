using Microsoft.AspNetCore.Mvc;
using System;

namespace RehabilitationSystem.EngineAPI.Controllers
{
    /// <summary>
    /// Kullanıcı kimlik doğrulama işlemlerini yöneten Controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Kullanıcı girişi yapar.
        /// </summary>
        /// <param name="request">Kullanıcı adı ve şifre bilgilerini içeren model.</param>
        /// <returns>Başarılı ise token, değilse hata mesajı döndürür.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Gerçek bir uygulamada bu bilgiler veritabanından kontrol edilmelidir.
                // Şimdilik sabit değerler ("grup11", "12345") ile kontrol sağlanıyor.
                if (request.Username == "grup11" && request.Password == "12345")
                {
                    // Başarılı giriş loglanır.
                    Console.WriteLine($"Başarılı giriş: {request.Username}");
                    return Ok(new { status = "ok", token = "mock-auth-token-grup11" });
                }
                else
                {
                    // Başarısız giriş denemesi loglanır.
                    Console.WriteLine($"Başarısız giriş denemesi: {request.Username}");
                    return Unauthorized(new { status = "error", message = "Geçersiz kullanıcı adı veya şifre." });
                }
            }
            catch (Exception ex)
            {
                // Beklenmedik bir hata oluşursa loglanır ve sunucu hatası döndürülür.
                Console.WriteLine($"Login sırasında hata: {ex.Message}");
                return StatusCode(500, new { status = "error", message = "Sunucuda bir hata oluştu." });
            }
        }
    }

    /// <summary>
    /// Login isteği için gelen verileri tutan model.
    /// </summary>
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
