import React from 'react';
import { View, Text } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import ControlSection from '../components/ControlSection';

/**
 * Aktif terapi bilgilerinin görüntülendiği ekran.
 * Bu ekranda, devam eden seansla ilgili canlı veriler yer alacaktır.
 */
const Therapy = () => {
  // Bu veriler ileride backend'den veya cihazdan anlık olarak alınacaktır.
  const therapyData = {
    patientName: 'Grup 11',
    duration: '15:30',
    totalDuration: '30:00',
    currentWeight: '25 kg',
    status: 'Devam Ediyor',
  };

  return (
    <View className="flex-1 bg-gray-50 items-center justify-center p-5">
      <ControlSection title="Aktif Terapi Bilgileri">
        <Ionicons name="body-outline" size={56} color="#5D3FD3" className="mb-4"/>
        <View className="w-full">
            <InfoRow label="Hasta Adı:" value={therapyData.patientName} />
            <InfoRow label="Geçen Süre:" value={`${therapyData.duration} / ${therapyData.totalDuration}`} />
            <InfoRow label="Ağırlık:" value={therapyData.currentWeight} />
            <InfoRow label="Durum:" value={therapyData.status} valueColor="text-green-500" />
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
