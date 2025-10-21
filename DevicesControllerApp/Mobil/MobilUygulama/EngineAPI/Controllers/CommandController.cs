using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için bu kütüphane gerekli.
using Microsoft.AspNetCore.Mvc;
using RehabilitationSystem.EngineAPI.Services;

namespace RehabilitationSystem.EngineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommandController : ControllerBase
    {
        /// <summary>
        /// Mobil uygulamadan gelen tüm komutları tek bir noktadan işler ve ilgili servise yönlendirir.
        /// </summary>
        /// <param name="request">Gelen komut bilgisini içeren JSON modeli.</param>
        /// <returns>İşlemin başarılı veya başarısız olduğunu belirten bir sonuç döner.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Command))
            {
                return BadRequest(new { status = "error", message = "Komut boş olamaz." });
            }
            Console.WriteLine($"Gelen komut: {request.Command}");

            try
            {
                switch (request.Command.ToLower())
                {
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
                        if (
                            !User.IsInRole("Admin")
                            && !User.IsInRole("Servis")
                            && !User.IsInRole("Operator")
                        )
                        {
                            return Forbid(); // Kullanıcının rolü bu işlem için yetersiz.
                        }
                        await Engine.EmergencyStop();
                        break;

                    case "up":
                        await Engine.MoveUp();
                        break;
                    case "down":
                        await Engine.MoveDown();
                        break;
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
                    case "weightdecrease": // Ağırlık Azaltma Azalt
                        await Engine.WeightDecrease();
                        break;

                    default:
                        return BadRequest(
                            new { status = "error", message = $"Geçersiz komut: {request.Command}" }
                        );
                }

                return Ok(new { status = "ok", received = request.Command });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Komut işlenirken hata oluştu: {ex.Message}");
                return StatusCode(
                    500,
                    new { status = "error", message = "Sunucuda bir hata oluştu." }
                );
            }
        }
    }

    /// <summary>
    /// API'ye mobil uygulamadan gönderilen komut isteği için model.
    /// </summary>
    public class CommandRequest
    {
        public string? Command { get; set; }
    }
}
