using Microsoft.AspNetCore.Mvc;
using RehabilitationSystem.EngineAPI.Services;

namespace RehabilitationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        [HttpPost("start")]
        public async Task<IActionResult> StartTherapy()
        {
            try
            {
                var envelope = await Engine.Start();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopTherapy()
        {
            try
            {
                var envelope = await Engine.Stop();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("pause")]
        public async Task<IActionResult> PauseTherapy()
        {
            try
            {
                var envelope = await Engine.Pause();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("resume")]
        public async Task<IActionResult> ResumeTherapy()
        {
            try
            {
                var envelope = await Engine.Resume();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("emergency")]
        public async Task<IActionResult> EmergencyStop()
        {
            try
            {
                var envelope = await Engine.EmergencyStop();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("up")]
        public async Task<IActionResult> MoveUp()
        {
            try
            {
                var envelope = await Engine.MoveUp();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("down")]
        public async Task<IActionResult> MoveDown()
        {
            try
            {
                var envelope = await Engine.MoveDown();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("left")]
        public async Task<IActionResult> MoveLeft()
        {
            try
            {
                var envelope = await Engine.MoveLeft();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("right")]
        public async Task<IActionResult> MoveRight()
        {
            try
            {
                var envelope = await Engine.MoveRight();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("footincrease")]
        public async Task<IActionResult> FootIncrease()
        {
            try
            {
                var envelope = await Engine.FootIncrease();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("footdecrease")]
        public async Task<IActionResult> FootDecrease()
        {
            try
            {
                var envelope = await Engine.FootDecrease();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("barup")]
        public async Task<IActionResult> BarUp()
        {
            try
            {
                var envelope = await Engine.BarUp();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("bardown")]
        public async Task<IActionResult> BarDown()
        {
            try
            {
                var envelope = await Engine.BarDown();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("weightincrease")]
        public async Task<IActionResult> WeightIncrease()
        {
            try
            {
                var envelope = await Engine.WeightIncrease();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }

        [HttpPost("weightdecrease")]
        public async Task<IActionResult> WeightDecrease()
        {
            try
            {
                var envelope = await Engine.WeightDecrease();
                
                if (envelope == null)
                {
                    return StatusCode(500, new { message = "Ana makineye ulaşılamadı.", error = Engine.LastError });
                }

                return Ok(envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Komut gönderilemedi.", error = ex.Message });
            }
        }
    }
}