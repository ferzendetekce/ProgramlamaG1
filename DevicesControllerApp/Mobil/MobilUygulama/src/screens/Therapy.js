import React, { useEffect, useState, useCallback, useRef } from "react";
import { View, Text, ActivityIndicator, StyleSheet, Animated, Easing } from "react-native";
import { Ionicons } from "@expo/vector-icons";
import ControlSection from "../components/ControlSection";
import statusService from "../services/statusService";

const Therapy = ({ navigation }) => {
  const [therapyData, setTherapyData] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const fadeAnim = useRef(new Animated.Value(0)).current;

  const loadTherapy = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const data = await statusService.fetchTherapy();
      setTherapyData(data);
    } catch (err) {
      console.error("Terapi bilgisi alınamadı:", err);
      setError(err.message || "Terapi bilgisi alınamadı.");
      if (err.status === 401 && navigation) {
        navigation.replace("Login");
      }
    } finally {
      setLoading(false);
    }
  }, [navigation]);

  useEffect(() => {
    loadTherapy();
    const intervalId = setInterval(loadTherapy, 5000);
    return () => clearInterval(intervalId);
  }, [loadTherapy]);

  useEffect(() => {
    Animated.timing(fadeAnim, {
      toValue: 1,
      duration: 450,
      easing: Easing.out(Easing.ease),
      useNativeDriver: true,
    }).start();
  }, [fadeAnim]);

  const isRunning = !!therapyData?.isRunning;

  const formatElapsed = () => {
    if (!isRunning || !therapyData?.startedAt || !therapyData?.lastUpdate) return "00:00";
    const start = new Date(therapyData.startedAt);
    const last = new Date(therapyData.lastUpdate);
    const diff = Math.max(0, last.getTime() - start.getTime());
    const minutes = Math.floor(diff / 60000);
    const seconds = Math.floor((diff % 60000) / 1000);
    const pad = (val) => String(val).padStart(2, "0");
    return `${pad(minutes)}:${pad(seconds)}`;
  };

  const formatTotal = () => {
    const total = therapyData?.targetDurationMinutes || 0;
    const pad = (val) => String(val).padStart(2, "0");
    return `${pad(total)}:00`;
  };

  const progressPercent = () => {
    if (!isRunning || !therapyData?.startedAt || !therapyData?.lastUpdate || !therapyData?.targetDurationMinutes)
      return 0;
    const start = new Date(therapyData.startedAt);
    const last = new Date(therapyData.lastUpdate);
    const diffMs = Math.max(0, last.getTime() - start.getTime());
    const targetMs = therapyData.targetDurationMinutes * 60000;
    return Math.min(100, Math.round((diffMs / targetMs) * 100));
  };

  const statusColor = therapyData?.isEmergency
    ? "#DC2626"
    : therapyData?.isRunning
    ? "#16A34A"
    : "#111827";

  return (
    <Animated.View
      style={[
        styles.container,
        { opacity: fadeAnim, transform: [{ translateY: fadeAnim.interpolate({ inputRange: [0, 1], outputRange: [16, 0] }) }] },
      ]}
    >
      <ControlSection title="Aktif Terapi Bilgileri">
        <View style={styles.headerRow}>
          <Ionicons name="body-outline" size={56} color="#5D3FD3" />
          <View>
            <Text style={styles.name}>{therapyData?.patientName || "Henüz başlatılmadı"}</Text>
            <Text style={[styles.status, { color: statusColor }]}>{therapyData?.statusText || "Hazır"}</Text>
          </View>
        </View>

        {error ? <Text style={styles.errorText}>{error}</Text> : null}

        {loading ? (
          <ActivityIndicator size="large" color="#5D3FD3" />
        ) : (
          <>
            <View style={styles.infoRow}>
              <Text style={styles.label}>Geçen Süre</Text>
              <Text style={styles.value}>{`${formatElapsed()} / ${formatTotal()}`}</Text>
            </View>
            <View style={styles.progressTrack}>
              <View style={[styles.progressFill, { width: `${progressPercent()}%` }]} />
            </View>
            <View style={styles.infoRow}>
              <Text style={styles.label}>Ağırlık Azaltma</Text>
              <Text style={styles.value}>{isRunning ? `${Math.round(therapyData?.weightSupport || 0)} kg` : "-"}</Text>
            </View>
            <View style={styles.infoRow}>
              <Text style={styles.label}>Ayak Numarası</Text>
              <Text style={styles.value}>{isRunning ? therapyData?.shoeSize || "-" : "-"}</Text>
            </View>
            <View style={styles.infoRow}>
              <Text style={styles.label}>Destek Barı</Text>
              <Text style={styles.value}>{isRunning ? `${therapyData?.supportBarHeight ?? 0} m` : "-"}</Text>
            </View>
          </>
        )}
      </ControlSection>
    </Animated.View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#EEF2F7",
    padding: 16,
  },
  headerRow: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 12,
  },
  name: {
    fontSize: 20,
    fontWeight: "700",
    color: "#111827",
    marginLeft: 12,
  },
  status: {
    fontSize: 14,
    fontWeight: "600",
    marginLeft: 12,
  },
  infoRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    marginBottom: 10,
  },
  label: {
    fontSize: 16,
    color: "#6B7280",
  },
  value: {
    fontSize: 16,
    fontWeight: "700",
    color: "#111827",
  },
  progressTrack: {
    height: 12,
    borderRadius: 8,
    backgroundColor: "#E5E7EB",
    marginBottom: 12,
    overflow: "hidden",
  },
  progressFill: {
    height: "100%",
    backgroundColor: "#5D3FD3",
  },
  errorText: {
    color: "#DC2626",
    marginBottom: 8,
  },
});

export default Therapy;
