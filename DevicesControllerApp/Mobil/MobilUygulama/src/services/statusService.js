import AsyncStorage from "@react-native-async-storage/async-storage";

const DEFAULT_PORT = "5086";

const fetchWithTimeout = async (url, options = {}, timeoutMs = 1200) => {
  const controller = new AbortController();
  const id = setTimeout(() => controller.abort(), timeoutMs);
  try {
    const res = await fetch(url, { ...options, signal: controller.signal });
    clearTimeout(id);
    return res;
  } catch (err) {
    clearTimeout(id);
    throw err;
  }
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

const getStoredHost = async () => {
  const raw = await AsyncStorage.getItem("apiHost");
  return raw ? JSON.parse(raw) : null;
};

const checkConnection = async (ip, port, timeoutMs = 1000) => {
  const url = `http://${ip}:${port}/api/status/ping`;
  const response = await fetchWithTimeout(url, {}, timeoutMs);
  const data = await response.json();
  if (!response.ok || data.status !== "ok") {
    throw new Error(data.message || "Sunucuya ulaşılamadı.");
  }
  return data;
};

const fetchTherapy = async () => {
  const host = await getStoredHost();
  if (!host) throw new Error("Sunucu ayarı bulunamadı.");
  const token = await getStoredToken();

  const url = `http://${host.ip}:${host.port}/api/status/therapy`;
  const response = await fetchWithTimeout(url, {
    headers: { Authorization: `Bearer ${token}` },
  }, 1200);

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
};

/**
 * Aynı subnet içinde (x.x.x.y) hızlı tarama yaparak ilk ulaşılabilen host'u döndürür.
 */
const autoDiscover = async () => {
  const stored = await getStoredHost().catch(() => null);
  const baseIp = (stored?.ip || "10.200.117.50").split(".");
  if (baseIp.length !== 4) throw new Error("IP formatı hatalı.");
  const subnet = `${baseIp[0]}.${baseIp[1]}.${baseIp[2]}`;
  const start = Math.max(2, parseInt(baseIp[3], 10) - 5);
  const end = Math.min(254, start + 20);

  const primary = [
    stored,
    { ip: "10.200.117.50", port: DEFAULT_PORT },
    { ip: "10.0.2.2", port: DEFAULT_PORT },
    { ip: "127.0.0.1", port: DEFAULT_PORT },
  ].filter(Boolean);

  for (const c of primary) {
    try {
      await checkConnection(c.ip, c.port, 800);
      return c;
    } catch {}
  }

  for (let i = start; i <= end; i++) {
    const candidate = { ip: `${subnet}.${i}`, port: DEFAULT_PORT };
    try {
      await checkConnection(candidate.ip, candidate.port, 700);
      return candidate;
    } catch {}
  }

  throw new Error("Hiçbir aday IP'ye ulaşılamadı. Manuel giriniz.");
};

const sendDisconnect = async () => {
  const host = await getStoredHost();
  if (!host) return;
  try {
    const token = await getStoredToken();
    await fetchWithTimeout(`http://${host.ip}:${host.port}/api/Command`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ command: "disconnect" }),
    }, 700);
  } catch (err) {
    console.warn("Disconnect isteği gönderilemedi:", err.message);
  }
};

export default {
  checkConnection,
  fetchTherapy,
  autoDiscover,
  sendDisconnect,
};
