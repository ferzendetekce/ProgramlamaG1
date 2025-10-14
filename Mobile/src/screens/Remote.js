import { ScrollView, View, Text } from 'react-native';
import React, { useEffect, useState } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import remoteService from '../services/remoteService';
import ControlButton from '../components/ControlButton';
import ControlSection from '../components/ControlSection';

/**
 * Cihaz ve terapi kontrollerinin yapıldığı ana kontrol ekranı.
 * Hız kontrol bölümü kaldırıldı.
 */
const Remote = ({ navigation }) => {
  const [host, setHost] = useState(null);
  const [isPaused, setIsPaused] = useState(false);

  // Ekran yüklendiğinde hafızadan bağlantı bilgilerini çeker.
  useEffect(() => {
    const loadHost = async () => {
      const value = await AsyncStorage.getItem("apiHost");
      if (value) {
        setHost(JSON.parse(value));
      } else {
        // Host bilgisi yoksa, kullanıcıyı en başa, bağlantı ekranına yönlendir.
        navigation.navigate("Connect");
      }
    };
    loadHost();
  }, []);

  // Backend'e komut gönderen merkezi fonksiyon.
  const sendCommand = async (command) => {
    if (host) {
      await remoteService.remoteService(host.ip, host.port, command);
    }
  };

  const handlePauseResume = () => {
    if (isPaused) {
      sendCommand("resume");
      setIsPaused(false);
    } else {
      sendCommand("pause");
      setIsPaused(true);
    }
  };

  return (
    <ScrollView contentContainerStyle={{ flexGrow: 1, alignItems: 'center', justifyContent: 'center' }} className="bg-gray-50 p-5 pt-12">
      
      {/* Terapi Kontrol Bölümü */}
      <ControlSection title="Terapi Kontrolleri">
        <View className="flex-row flex-wrap justify-center">
          <ControlButton onPress={() => sendCommand("start")} text="Başlat" colorClass="bg-emerald-500" />
          <ControlButton onPress={handlePauseResume} text={isPaused ? "Devam" : "Beklet"} colorClass="bg-amber-500" />
          <ControlButton onPress={() => sendCommand("stop")} text="Durdur" colorClass="bg-rose-500" />
          <ControlButton onPress={() => sendCommand("emergencystop")} text="Acil Stop" colorClass="bg-red-700" />
        </View>
      </ControlSection>

      {/* Vinç Kontrol Bölümü */}
      <ControlSection title="Vinç Kontrolleri">
        <ControlButton onPress={() => sendCommand("up")} icon="arrow-up" colorClass="bg-indigo-500" size="large" />
        <View className="flex-row items-center">
          <ControlButton onPress={() => sendCommand("left")} icon="arrow-back" colorClass="bg-indigo-500" size="large" />
          <View className="w-24 h-24" />
          <ControlButton onPress={() => sendCommand("right")} icon="arrow-forward" colorClass="bg-indigo-500" size="large" />
        </View>
        <ControlButton onPress={() => sendCommand("down")} icon="arrow-down" colorClass="bg-indigo-500" size="large" />
      </ControlSection>

      {/* Cihaz Ayar Bölümü */}
      <ControlSection title="Cihaz Ayarları">
        <View className="flex-row justify-between items-center w-full mb-4">
            <ControlButton onPress={() => sendCommand("footdecrease")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
            <Text className="text-lg font-semibold text-gray-700">Ayak Numarası</Text>
            <ControlButton onPress={() => sendCommand("footincrease")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
         <View className="flex-row justify-between items-center w-full mb-4">
            <ControlButton onPress={() => sendCommand("bardown")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
            <Text className="text-lg font-semibold text-gray-700">Destek Barı</Text>
            <ControlButton onPress={() => sendCommand("barup")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
         <View className="flex-row justify-between items-center w-full">
            <ControlButton onPress={() => sendCommand("weightdecrease")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
            <Text className="text-lg font-semibold text-gray-700">Ağırlık Azaltma</Text>
            <ControlButton onPress={() => sendCommand("weightincrease")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
      </ControlSection>

    </ScrollView>
  );
};

export default Remote;

