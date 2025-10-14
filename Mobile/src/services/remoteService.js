/**
 * Cihaz ve terapi komutlarını backend'e göndermek için kullanılan servis.
 * 'speed' parametresi kaldırıldı.
 */
const remoteService = async (ip, port, command) => {
  try {
    // Gönderilen komutu konsola logla.
    console.log('Komut gönderiliyor:', { command });
    const apiUrl = `http://${ip}:${port}/api/Command`

    // Fetch API kullanarak backend'e POST isteği gönder.
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ command: command }) // Sadece komutu JSON formatında gönder.
    });

    // HTTP yanıtı başarılı değilse (2xx aralığı dışında ise) hata fırlat.
    if (!response.ok) {
      throw new Error("API'ye post isteği başarısız oldu.");
    }

    // Başarılı yanıtı JSON olarak işle ve konsola yazdır.
    const data = await response.json(); 
    console.log("Backend yanıtı:", data);
    return data;
  }
  catch (error) {
    // Herhangi bir hata durumunda konsola hata mesajını yazdır.
    console.error("Komut gönderilemedi:", error);
  }
}

export default {
  remoteService,
};

