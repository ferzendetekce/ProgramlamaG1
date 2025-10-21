// src/services/authService.js

/**
 * Kullanıcı kimlik doğrulama işlemleri için backend ile iletişim kuran servis.
 */
 const login = async (ip, port, username, password) => {
  try {
    // Backend API'nin login endpoint'i (HTTPS düşünülerek https:// yazılabilir ama geliştirme için http:// daha kolay)
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
      throw new Error(data.message || `Giriş başarısız: ${response.status}`);
    }

    if (data.status === 'ok' && data.token) {
        console.log("Giriş başarılı, token alındı.");
        return data.token;
    } else {
        throw new Error("API'den geçersiz yanıt formatı alındı.");
    }

  } catch (error) {
    console.error("Giriş sırasında hata:", error);
    throw error; 
  }
};

export default {
  login,
};