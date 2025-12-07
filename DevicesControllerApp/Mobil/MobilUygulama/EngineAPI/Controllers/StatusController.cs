using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RehabilitationSystem.EngineAPI.Services;

namespace RehabilitationSystem.EngineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        /// <summary>
        /// Mobil tarafının API ve ana forma erişebilirliğini kontrol eder.
        /// </summary>
        [HttpGet("ping")]
        [AllowAnonymous]
        public async Task<IActionResult> Ping()
        {
            try
            {
                var reachable = await Engine.CheckConnectionAsync();
                return Ok(new { status = "ok", reachable });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Ana forma ulaşılamadı.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Aktif terapi bilgisini döndürür.
        /// </summary>
        [HttpGet("therapy")]
        [Authorize]
        public async Task<IActionResult> Therapy()
        {
            try
            {
                var snapshot = await Engine.GetTherapySnapshotAsync();
                if (snapshot == null)
                {
                    return StatusCode(503, new { status = "error", message = Engine.LastError ?? "Ana form yanıt vermiyor." });
                }

                return Ok(new { status = "ok", therapy = snapshot });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Terapi bilgisi alınamadı.", detail = ex.Message });
            }
        }
    }
}
