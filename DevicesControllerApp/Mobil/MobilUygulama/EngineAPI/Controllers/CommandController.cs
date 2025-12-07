using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RehabilitationSystem.EngineAPI.Services;

namespace RehabilitationSystem.EngineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommandController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Command))
            {
                return BadRequest(new { status = "error", message = "Komut boş olamaz." });
            }

            var incomingCommand = request.Command.Trim().ToLower();
            Console.WriteLine($"Gelen komut: {incomingCommand}");

            if (!PermissionMatrix.IsAllowed(User, incomingCommand, out var reason))
            {
                return StatusCode(403, new { status = "error", message = reason });
            }

            try
            {
                var envelope = incomingCommand switch
                {
                    "start" => await Engine.Start(),
                    "stop" => await Engine.Stop(),
                    "pause" => await Engine.Pause(),
                    "resume" => await Engine.Resume(),
                    "emergencystop" => await Engine.EmergencyStop(),
                    "disconnect" => await Engine.Stop(),
                    "up" => await Engine.MoveUp(),
                    "down" => await Engine.MoveDown(),
                    "footincrease" => await Engine.FootIncrease(),
                    "footdecrease" => await Engine.FootDecrease(),
                    "barup" => await Engine.BarUp(),
                    "bardown" => await Engine.BarDown(),
                    "weightincrease" => await Engine.WeightIncrease(),
                    "weightdecrease" => await Engine.WeightDecrease(),
                    _ => null
                };

                if (envelope == null)
                {
                    var errorMessage = Engine.LastError ?? "Ana forma ulaşılamadı.";
                    return StatusCode(503, new { status = "error", message = errorMessage });
                }

                return Ok(new { status = "ok", received = incomingCommand, therapy = envelope.Therapy });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Komut işlenirken hata oluştu: {ex.Message}");
                var message = Engine.LastError ?? "Sunucuda bir hata oluştu.";
                return StatusCode(500, new { status = "error", message });
            }
        }
    }

    public class CommandRequest
    {
        public string? Command { get; set; }
    }
}
