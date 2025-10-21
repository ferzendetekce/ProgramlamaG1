import { ScrollView, View, Text, Alert } from 'react-native';
import React, { useEffect, useState } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import remoteService from '../services/remoteService';
import ControlButton from '../components/ControlButton';
import ControlSection from '../components/ControlSection';

// Güncellenmiş bildirim fonksiyonlarını import ediyoruz
import {
  sendImmediateNotification,
  sendErrorNotification,
  cancelAllNotifications
} from '../services/notificationService';

const Remote = ({ navigation }) => {
  const [host, setHost] = useState(null);
  const [isPaused, setIsPaused] = useState(false);

  useEffect(() => {
    const loadHost = async () => {
      const value = await AsyncStorage.getItem("apiHost");
      if (value) {
        setHost(JSON.parse(value));
      } else {
        navigation.navigate("Connect");
      }
    };
    loadHost();
  }, [navigation]);

  // Backend'e komut gönderen ve bildirim yollayan merkezi fonksiyon.
  const sendCommandWithNotification = async (command, notificationTitle, notificationBody) => {
    if (!host) {
      Alert.alert("Hata", "Sunucu bağlantı bilgileri bulunamadı.");
      return;
    }
    try {
      await remoteService.remoteService(host.ip, host.port, command);
      // Komut başarılı olursa bildirimi gönder
      sendImmediateNotification(notificationTitle, notificationBody);
    } catch (error) {
      console.error(`'${command}' komutu gönderilirken hata:`, error);
      sendErrorNotification('Cihaza komut gönderilemedi.');
      if (error.message.includes("401")) {
        Alert.alert(
          "Oturum Hatası",
          "Oturumunuzun süresi dolmuş. Lütfen tekrar giriş yapın.",
          [{ text: "Tamam", onPress: () => navigation.replace('Login') }]
        );
      } else {
        Alert.alert("Hata", error.message);
      }
    }
  };

  const handlePauseResume = () => {
    if (isPaused) {
      sendCommandWithNotification("resume", "Terapi Devam Ediyor", "Seans kaldığı yerden devam ettirildi.");
      setIsPaused(false);
    } else {
      sendCommandWithNotification("pause", "Terapi Bekletildi", "Seans geçici olarak duraklatıldı.");
      setIsPaused(true);
    }
  };

  return (
    <ScrollView contentContainerStyle={{ flexGrow: 1, alignItems: 'center', justifyContent: 'center' }} className="bg-gray-50 p-5 pt-12">

      <ControlSection title="Terapi Kontrolleri">
        <View className="flex-row flex-wrap justify-center">
          <ControlButton onPress={() => sendCommandWithNotification("start", "Terapi Başlatıldı", "Yeni bir terapi seansı başladı.")} text="Başlat" colorClass="bg-emerald-500" />
          <ControlButton onPress={handlePauseResume} text={isPaused ? "Devam" : "Beklet"} colorClass="bg-amber-500" />
          <ControlButton onPress={() => { sendCommandWithNotification("stop", "Terapi Durduruldu", "Terapi seansı sonlandırıldı."); cancelAllNotifications(); }} text="Durdur" colorClass="bg-rose-500" />
          <ControlButton onPress={() => { sendCommandWithNotification("emergencystop", "ACİL DURUM", "Tüm sistemler acil durum modunda durduruldu!"); cancelAllNotifications(); }} text="Acil Stop" colorClass="bg-red-700" />
        </View>
      </ControlSection>

      <ControlSection title="Vinç Kontrolleri">
        <ControlButton onPress={() => sendCommandWithNotification("up", "Vinç Kontrolü", "Vinç yukarı hareket ettirildi.")} icon="arrow-up" colorClass="bg-indigo-500" size="large" />
        <View style={{ height: 20 }} />
        <ControlButton onPress={() => sendCommandWithNotification("down", "Vinç Kontrolü", "Vinç aşağı hareket ettirildi.")} icon="arrow-down" colorClass="bg-indigo-500" size="large" />
      </ControlSection>

      <ControlSection title="Cihaz Ayarları">
        <View className="flex-row justify-between items-center w-full mb-4">
            <ControlButton onPress={() => sendCommandWithNotification("footdecrease", "Ayar Değişikliği", "Ayak numarası küçültüldü.")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
            <Text className="text-lg font-semibold text-gray-700">Ayak Numarası</Text>
            <ControlButton onPress={() => sendCommandWithNotification("footincrease", "Ayar Değişikliği", "Ayak numarası büyütüldü.")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
         <View className="flex-row justify-between items-center w-full mb-4">
            <ControlButton onPress={() => sendCommandWithNotification("bardown", "Ayar Değişikliği", "Destek barı alçaltıldı.")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
            <Text className="text-lg font-semibold text-gray-700">Destek Barı</Text>
            <ControlButton onPress={() => sendCommandWithNotification("barup", "Ayar Değişikliği", "Destek barı yükseltildi.")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
         <View className="flex-row justify-between items-center w-full">
            <ControlButton onPress={() => sendCommandWithNotification("weightdecrease", "Ayar Değişikliği", "Ağırlık azaltma düşürüldü.")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
            <Text className="text-lg font-semibold text-gray-700">Ağırlık Azaltma</Text>
            <ControlButton onPress={() => sendCommandWithNotification("weightincrease", "Ayar Değişikliği", "Ağırlık azaltma artırıldı.")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
      </ControlSection>

    </ScrollView>
  );
};

export default Remote;

