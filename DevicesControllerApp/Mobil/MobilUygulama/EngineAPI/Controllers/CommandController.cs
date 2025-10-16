using RehabilitationSystem.EngineAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RehabilitationSystem.EngineAPI.Controllers
{
    /// <summary>
    /// Cihaz ve terapi komutlarını asenkron olarak yöneten API Controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommandController : ControllerBase
    {
        /// <summary>
        /// Mobil uygulamadan gelen komutları asenkron olarak işler ve ilgili servise yönlendirir.
        /// </summary>
        /// <param name="request">Komut bilgisini içeren model.</param>
        /// <returns>İşlem sonucunu döndürür.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommandRequest request)
        {
            try
            {
                Console.WriteLine($"Gelen komut: {request.Command}");
                switch (request.Command?.ToLower())
                {
                    // Terapi Kontrolleri
                    case "start":
                        await Engine.Start();
                        break;
                    case "stop":
                        await Engine.Stop();
                        break;
                    case "pause":
                        await Engine.Pause();
                        break;
                    case "resume":
                        await Engine.Resume();
                        break;
                    case "emergencystop":
                        await Engine.EmergencyStop();
                        break;

                    // Vinç Kontrolleri
                    case "up":
                        await Engine.MoveUp();
                        break;
                    case "down":
                        await Engine.MoveDown();
                        break;
                    case "left":
                        await Engine.MoveLeft();
                        break;
                    case "right":
                        await Engine.MoveRight();
                        break;

                    // Cihaz Ayar Kontrolleri
                    case "footincrease":
                        await Engine.FootIncrease();
                        break;
                    case "footdecrease":
                        await Engine.FootDecrease();
                        break;
                    case "barup":
                        await Engine.BarUp();
                        break;
                    case "bardown":
                        await Engine.BarDown();
                        break;
                    case "weightincrease":
                        await Engine.WeightIncrease();
                        break;
                    case "weightdecrease":
                        await Engine.WeightDecrease();
                        break;
                        
                    default:
                        // Geçersiz komut durumunda hata döndürülür.
                        return BadRequest(new { status = "error", message = "Geçersiz komut" });
                }
                
                // Başarılı işlem sonucunda durumu ve alınan komutu geri döndürür.
                return Ok(new { status = "ok", received = request.Command });
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılır ve sunucu hatası döndürülür.
                Console.WriteLine($"Komut işlenirken hata oluştu: {ex.Message}");
                return StatusCode(500, new { status = "error", message = "Sunucuda bir hata oluştu." });
            }
        }
    }

    /// <summary>
    /// API'ye gönderilen komut isteği için model. 'Speed' parametresi kaldırıldı.
    /// </summary>
    public class CommandRequest
    {
        public string? Command { get; set; }
    }
}

