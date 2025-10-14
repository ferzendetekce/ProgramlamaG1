/**
 * Kullanıcı kimlik doğrulama (authentication) işlemleri için backend ile iletişim kuran servis.
 */

const login = async (ip, port, username, password) => {
  try {
    const apiUrl = `http://${ip}:${port}/api/Auth/login`;
    console.log(`Giriş yapılıyor: ${apiUrl} kullanıcı: ${username}`);

    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ username, password }),
    });

    const data = await response.json();

    if (!response.ok) {
      // Sunucudan gelen hata mesajını kullan, yoksa genel bir mesaj göster.
      throw new Error(data.message || "Giriş başarısız oldu.");
    }

    console.log("Giriş başarılı:", data);
    return data; // Başarılı yanıtta { status: "ok", token: "..." } döner.
  } catch (error) {
    // Hata oluşursa konsola yazdır ve hatayı UI katmanına fırlat.
    console.error("Giriş sırasında hata:", error);
    throw error;
  }
};

export default {
  login,
};
