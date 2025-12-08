import AsyncStorage from "@react-native-async-storage/async-storage";
import * as Network from "expo-network";

const DEFAULT_PORT = "5086";

const fetchWithTimeout = async (url, options = {}, timeoutMs = 2500) => {
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
    const err = new Error("Oturum dogrulamasi bulunamadi.");
    err.status = 401;
    throw err;
  }
  return token;
};

const getStoredHost = async () => {
  const raw = await AsyncStorage.getItem("apiHost");
  return raw ? JSON.parse(raw) : null;
};

const checkConnection = async (ip, port, timeoutMs = 1500) => {
  const url = `http://${ip}:${port}/api/status/ping`;
  const response = await fetchWithTimeout(url, {}, timeoutMs);
  const data = await response.json();
  if (!response.ok || data.status !== "ok") {
    throw new Error(data.message || "Sunucuya ulasilamadi.");
  }
  return data;
};

const fetchTherapy = async () => {
  const host = await getStoredHost();
  if (!host) throw new Error("Sunucu ayari bulunamadi.");
  const token = await getStoredToken();

  const url = `http://${host.ip}:${host.port}/api/status/therapy`;
  const response = await fetchWithTimeout(
    url,
    {
      headers: { Authorization: `Bearer ${token}` },
    },
    2500
  );

  const data = await response.json();
  if (response.status === 401) {
    const err = new Error("Oturumunuzun suresi doldu.");
    err.status = 401;
    throw err;
  }
  if (!response.ok || data.status !== "ok") {
    throw new Error(data.message || "Terapi bilgisi alinmadi.");
  }
  return data.therapy;
};

const autoDiscover = async () => {
  const stored = await getStoredHost().catch(() => null);
  let subnet = null;
  try {
    const ip = await Network.getIpAddressAsync();
    if (ip) {
      const parts = ip.split(".");
      if (parts.length === 4) subnet = `${parts[0]}.${parts[1]}.${parts[2]}`;
    }
  } catch {}

  const primary = [
    stored,
    subnet ? { ip: `${subnet}.1`, port: DEFAULT_PORT } : null,
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

  const candidates = [];
  if (subnet) {
    for (let i = 0; i <= 255; i++) {
      candidates.push({ ip: `${subnet}.${i}`, port: DEFAULT_PORT });
    }
  }

  for (let i = 0; i < candidates.length; i += 25) {
    const batch = candidates.slice(i, i + 25);
    const results = await Promise.all(
      batch.map(async (c) => {
        try {
          await checkConnection(c.ip, c.port, 600);
          return c;
        } catch {
          return null;
        }
      })
    );
    const found = results.find(Boolean);
    if (found) return found;
  }

  throw new Error("Hicbir aday IP'ye ulasilamadi. Manuel giriniz.");
};

export default {
  checkConnection,
  fetchTherapy,
  autoDiscover,
};
