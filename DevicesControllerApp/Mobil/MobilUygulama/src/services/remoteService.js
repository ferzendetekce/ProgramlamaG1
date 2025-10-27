
import AsyncStorage from '@react-native-async-storage/async-storage';

/**
 * @returns {Promise<string|null>}
 */
const getToken = async () => {
    try {
        return await AsyncStorage.getItem("authToken");
    } catch (e) {
        console.error("Token okunurken hata oluştu:", e);
        return null;
    }
};

/**
 * @param {string} ip - Backend API'sinin IP adresi.
 * @param {string} port - Backend API'sinin port numarası.
 * @param {string} command - Gönderilecek komut.
 * @returns {Promise<any>} API'den dönen başarılı JSON yanıtı.
 * @throws {Error}
 */
const remoteService = async (ip, port, command) => {
  const token = await getToken();

  if (!token) {
    console.error("Yetkilendirme token'ı bulunamadı. Kullanıcı giriş yapmamış veya oturum süresi dolmuş olabilir.");
    throw new Error("Oturum bulunamadı. Lütfen tekrar giriş yapın.");
  }

  const apiUrl = `http://${ip}:${port}/api/Command`;
  console.log(`'${command}' komutu, token ile ${apiUrl} adresine gönderiliyor...`);

  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}` 
      },
      body: JSON.stringify({ command: command })
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: `API isteği başarısız oldu. Durum Kodu: ${response.status}` }));
      throw new Error(errorData.message || "Bilinmeyen bir API hatası oluştu.");
    }

    const data = await response.json(); 
    console.log("Backend'den başarılı yanıt alındı:", data);
    return data;

  } catch (error) {
    console.error(`'${command}' komutu gönderilemedi:`, error);
    throw error;
  }
};

export default {
  remoteService,
};
