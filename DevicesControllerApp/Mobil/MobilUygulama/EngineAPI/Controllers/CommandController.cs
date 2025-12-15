using Microsoft.AspNetCore.Mvc;
using RehabilitationSystem.Communication; // DeviceCommunication'ın olduğu namespace
// using RehabilitationSystem.Models; // Therapy modelinin olduğu namespace

namespace RehabilitationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        // Global servis veya static değişkene erişim (Proje yapına göre değişebilir)
        // Örnek: TherapyService.CurrentTherapy
        
        [HttpPost("start")]
        public IActionResult StartTherapy()
        {
            // 1. Cihaza 'Başla' komutunu gönder
            bool commandSent = DeviceCommunication.Instance.StartTherapy();

            if (commandSent)
            {
                // *** KRİTİK DEĞİŞİKLİK ***
                // Cihazdan yanıt gelmesini beklemeden API'deki durumu hemen güncelle!
                // Böylece mobil uygulama "isRunning: true" görür.
                
                // Buradaki 'CurrentTherapyService' senin projendeki terapi durumunu tutan static sınıf veya servis olmalı.
                if (TherapyService.CurrentTherapy != null)
                {
                    TherapyService.CurrentTherapy.IsRunning = true;
                    TherapyService.CurrentTherapy.StatusText = "Terapi Başladı";
                    TherapyService.CurrentTherapy.StartedAt = DateTime.Now;
                    TherapyService.CurrentTherapy.IsPaused = false;
                    TherapyService.CurrentTherapy.IsEmergency = false;
                }
            }
            else
            {
                return StatusCode(500, new { message = "Cihaza komut gönderilemedi." });
            }

            // Güncellenmiş nesneyi geri döndür
            return Ok(new 
            { 
                received = "start", 
                status = "ok", 
                therapy = TherapyService.CurrentTherapy 
            });
        }

        [HttpPost("stop")]
        public IActionResult StopTherapy()
        {
            bool commandSent = DeviceCommunication.Instance.StopTherapy();
            
            if (commandSent && TherapyService.CurrentTherapy != null)
            {
                // Durdurma işleminde de manuel güncelleme yapalım
                TherapyService.CurrentTherapy.IsRunning = false;
                TherapyService.CurrentTherapy.StatusText = "Durduruldu";
            }

            return Ok(new 
            { 
                received = "stop", 
                status = "ok", 
                therapy = TherapyService.CurrentTherapy 
            });
        }
        
        // Diğer komutlar (up, down vb.) burada kalabilir...
    }
}