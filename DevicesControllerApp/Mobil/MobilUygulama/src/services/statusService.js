import AsyncStorage from "@react-native-async-storage/async-storage";

const getStoredHost = async () => {
  const raw = await AsyncStorage.getItem("apiHost");
  if (!raw) {
    throw new Error("Bağlantı bilgisi bulunamadı. Önce 'Bağlan' ekranını kullanın.");
  }
  return JSON.parse(raw);
};

const getStoredToken = async () => {
  const token = await AsyncStorage.getItem("authToken");
  if (!token) {
    const err = new Error("Oturum doğrulaması bulunamadı.");
    err.status = 401;
    throw err;
  }
  return token;
};

/**
 * API'nin ve ana formun ayakta olup olmadığını kontrol eder.
 */
const checkConnection = async (ip, port) => {
  try {
    const url = `http://${ip}:${port}/api/status/ping`;
    const response = await fetch(url);
    const data = await response.json();
    if (!response.ok || data.status !== "ok") {
      throw new Error(data.message || "Sunucuya ulaşılamadı.");
    }
    return data;
  } catch (error) {
    console.error("Ping isteği başarısız:", error);
    throw error;
  }
};

/**
 * Aktif terapi bilgilerini backend'den çeker.
 */
const fetchTherapy = async () => {
  try {
    const host = await getStoredHost();
    const token = await getStoredToken();

    const url = `http://${host.ip}:${host.port}/api/status/therapy`;
    const response = await fetch(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    const data = await response.json();
    if (response.status === 401) {
      const err = new Error("Oturumunuzun süresi doldu.");
      err.status = 401;
      throw err;
    }

    if (!response.ok || data.status !== "ok") {
      throw new Error(data.message || "Terapi bilgisi alınamadı.");
    }

    return data.therapy;
  } catch (error) {
    console.error("Terapi bilgisi alınırken hata:", error);
    throw error;
  }
};

export default {
  checkConnection,
  fetchTherapy,
};
