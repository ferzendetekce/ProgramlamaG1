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
const fetchWithTimeout = async (url, timeoutMs = 1200) => {
  const controller = new AbortController();
  const id = setTimeout(() => controller.abort(), timeoutMs);
  try {
    const response = await fetch(url, { signal: controller.signal });
    clearTimeout(id);
    return response;
  } catch (err) {
    clearTimeout(id);
    throw err;
  }
};

const checkConnection = async (ip, port, timeoutMs = 1200) => {
  try {
    const url = `http://${ip}:${port}/api/status/ping`;
    const response = await fetchWithTimeout(url, timeoutMs);
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
  autoDiscover,
};

/**
 * Basit HTTP ping ile otomatik keşif: bilinen aday IP listesinde ilk döneni verir.
 */
async function autoDiscover() {
  const stored = await AsyncStorage.getItem("apiHost").then((v) => (v ? JSON.parse(v) : null)).catch(() => null);
  const baseIp = (stored?.ip || "10.200.117.50").split(".");
  if (baseIp.length !== 4) {
    throw new Error("IP formatı hatalı, manuel giriniz.");
  }
  const subnet = `${baseIp[0]}.${baseIp[1]}.${baseIp[2]}`;
  const start = Math.max(2, parseInt(baseIp[3], 10) - 5);
  const end = Math.min(254, start + 20);

  // İlk önce mevcut kayıtlı IP'yi dene
  const primaryCandidates = [
    stored,
    { ip: "10.200.117.50", port: "5086" },
    { ip: "10.0.2.2", port: "5086" },
    { ip: "127.0.0.1", port: "5086" },
  ].filter(Boolean);

  for (const c of primaryCandidates) {
    try {
      await checkConnection(c.ip, c.port, 900);
      return c;
    } catch {}
  }

  // Ardından aynı subnet'te lastOctet iterasyonu
  for (let i = start; i <= end; i++) {
    const candidate = { ip: `${subnet}.${i}`, port: "5086" };
    try {
      await checkConnection(candidate.ip, candidate.port, 700);
      return candidate;
    } catch {
      // hızlı geç
    }
  }

  throw new Error("Hiçbir aday IP'ye ulaşılamadı. Manuel giriniz.");
}
