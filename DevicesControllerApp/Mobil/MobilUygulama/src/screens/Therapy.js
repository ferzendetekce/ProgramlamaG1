import React, { useCallback, useEffect, useState } from 'react';
import { View, Text, ActivityIndicator } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import ControlSection from '../components/ControlSection';
import PButton from '../components/PButton';
import statusService from '../services/statusService';

/**
 * Aktif terapi bilgilerinin görüntülendiği ekran.
 */
const Therapy = ({ navigation }) => {
  const [therapyData, setTherapyData] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const loadTherapy = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await statusService.fetchTherapy();
      setTherapyData(data);
    } catch (err) {
      console.error("Terapi bilgisi alınamadı:", err);
      setError(err.message || "Terapi bilgisi alınamadı.");
      if (err.status === 401 && navigation) {
        navigation.replace('Login');
      }
    } finally {
      setLoading(false);
    }
  }, [navigation]);

  useEffect(() => {
    loadTherapy();
    const intervalId = setInterval(loadTherapy, 10000);
    return () => clearInterval(intervalId);
  }, [loadTherapy]);

  const formatElapsed = () => {
    if (!therapyData?.startedAt || !therapyData?.lastUpdate) return "00:00";
    const start = new Date(therapyData.startedAt);
    const last = new Date(therapyData.lastUpdate);
    const diff = Math.max(0, last.getTime() - start.getTime());
    const minutes = Math.floor(diff / 60000);
    const seconds = Math.floor((diff % 60000) / 1000);
    const pad = (val) => String(val).padStart(2, '0');
    return `${pad(minutes)}:${pad(seconds)}`;
  };

  const formatTotal = () => {
    const total = therapyData?.targetDurationMinutes || 0;
    const pad = (val) => String(val).padStart(2, '0');
    return `${pad(total)}:00`;
  };

  const statusColor = therapyData?.isEmergency
    ? "text-red-600"
    : therapyData?.isRunning
      ? "text-green-600"
      : "text-gray-800";

  return (
    <View className="flex-1 bg-gray-50 items-center justify-center p-5">
      <ControlSection title="Aktif Terapi Bilgileri">
        <Ionicons name="body-outline" size={56} color="#5D3FD3" className="mb-4"/>
        <View className="w-full">
          {error ? (
            <Text className="text-center text-red-600 mb-3">{error}</Text>
          ) : null}

          {loading ? (
            <ActivityIndicator size="large" color="#5D3FD3" />
          ) : (
            <>
              <InfoRow label="Hasta Adı:" value={therapyData?.patientName || 'Bilinmiyor'} />
              <InfoRow label="Geçen Süre:" value={`${formatElapsed()} / ${formatTotal()}`} />
              <InfoRow label="Ağırlık Azaltma:" value={`${Math.round(therapyData?.weightSupport || 0)} kg`} />
              <InfoRow label="Ayak Numarası:" value={therapyData?.shoeSize || '-'} />
              <InfoRow label="Destek Barı:" value={`${therapyData?.supportBarHeight ?? 0} m`} />
              <InfoRow label="Durum:" value={therapyData?.statusText || 'Hazır'} valueColor={statusColor} />
            </>
          )}
        </View>

        <View className="w-full mt-4">
          <PButton onPress={loadTherapy} disabled={loading}>Yenile</PButton>
        </View>
      </ControlSection>
    </View>
  );
};

// Bilgi satırlarını göstermek için yardımcı bir bileşen.
const InfoRow = ({ label, value, valueColor = 'text-gray-800' }) => (
    <View className="flex-row justify-between mb-4 pb-2 border-b border-gray-200">
        <Text className="text-lg text-gray-500">{label}</Text>
        <Text className={`text-lg font-semibold ${valueColor}`}>{value}</Text>
    </View>
);

export default Therapy;
