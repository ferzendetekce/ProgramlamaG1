import { ScrollView, View, Text, Alert, StyleSheet, Animated, Easing } from 'react-native';
import React, { useEffect, useRef, useState } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import remoteService from '../services/remoteService';
import ControlButton from '../components/ControlButton';
import ControlSection from '../components/ControlSection';
import {
  sendImmediateNotification,
  sendErrorNotification,
  cancelAllNotifications
} from '../services/notificationService';

const Remote = ({ navigation }) => {
  const [host, setHost] = useState(null);
  const [isPaused, setIsPaused] = useState(false);
  const fadeAnim = useRef(new Animated.Value(0)).current;
  const [errorText, setErrorText] = useState('');

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

  useEffect(() => {
    Animated.timing(fadeAnim, {
      toValue: 1,
      duration: 450,
      easing: Easing.out(Easing.ease),
      useNativeDriver: true,
    }).start();
  }, [fadeAnim]);

  const sendCommandWithNotification = async (command, notificationTitle, notificationBody) => {
    if (!host) {
      Alert.alert("Hata", "Sunucu bağlantı bilgileri bulunamadı.");
      return;
    }
    try {
      await remoteService.remoteService(host.ip, host.port, command);
      sendImmediateNotification(notificationTitle, notificationBody);
      setErrorText('');
    } catch (error) {
      console.error(`'${command}' komutu gönderilirken hata:`, error);
      sendErrorNotification('Cihaza komut gönderilemedi.');
      setErrorText(error.message || 'Bilinmeyen hata');
      if (error.status === 401) {
        Alert.alert(
          "Oturum Hatası",
          "Oturumunuzun süresi dolmuş olabilir. Lütfen tekrar giriş yapın.",
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
    <Animated.ScrollView contentContainerStyle={styles.container} style={{ opacity: fadeAnim, transform: [{ translateY: fadeAnim.interpolate({ inputRange: [0,1], outputRange: [16,0] }) }] }}>
      <View style={styles.hostCard}>
        <Text style={styles.hostTitle}>Sunucu</Text>
        <Text style={styles.hostValue}>{host ? `${host.ip}:${host.port}` : "Bağlı değil"}</Text>
      </View>

      <ControlSection title="Terapi Kontrolleri">
        {errorText ? <Text style={styles.inlineError}>{errorText}</Text> : null}
        <View style={styles.rowWrap}>
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
        <View style={styles.settingsRow}>
          <ControlButton onPress={() => sendCommandWithNotification("footdecrease", "Ayar Değişikliği", "Ayak numarası küçültüldü.")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
          <Text style={styles.settingLabel}>Ayak Numarası</Text>
          <ControlButton onPress={() => sendCommandWithNotification("footincrease", "Ayar Değişikliği", "Ayak numarası büyütüldü.")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
        <View style={styles.settingsRow}>
          <ControlButton onPress={() => sendCommandWithNotification("bardown", "Ayar Değişikliği", "Destek barı alçaltıldı.")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
          <Text style={styles.settingLabel}>Destek Barı</Text>
          <ControlButton onPress={() => sendCommandWithNotification("barup", "Ayar Değişikliği", "Destek barı yükseltildi.")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
        <View style={styles.settingsRow}>
          <ControlButton onPress={() => sendCommandWithNotification("weightdecrease", "Ayar Değişikliği", "Ağırlık azaltma düşürüldü.")} icon="remove" colorClass="bg-gray-300" textClass="text-black" />
          <Text style={styles.settingLabel}>Ağırlık Azaltma</Text>
          <ControlButton onPress={() => sendCommandWithNotification("weightincrease", "Ayar Değişikliği", "Ağırlık azaltma artırıldı.")} icon="add" colorClass="bg-gray-300" textClass="text-black" />
        </View>
      </ControlSection>
    </Animated.ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flexGrow: 1,
    padding: 16,
    paddingTop: 24,
    backgroundColor: '#EEF2F7'
  },
  hostCard: {
    backgroundColor: '#FFFFFF',
    padding: 14,
    borderRadius: 10,
    marginBottom: 14,
    shadowColor: '#000',
    shadowOpacity: 0.05,
    shadowOffset: { width: 0, height: 4 },
    shadowRadius: 8,
    elevation: 3,
    borderWidth: 1,
    borderColor: '#E5E7EB'
  },
  hostTitle: {
    fontSize: 12,
    color: '#6B7280',
    letterSpacing: 0.5
  },
  hostValue: {
    fontSize: 16,
    fontWeight: '700',
    color: '#111827'
  },
  rowWrap: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'center'
  },
  settingsRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    width: '100%',
    marginBottom: 12
  },
  settingLabel: {
    fontSize: 16,
    fontWeight: '600',
    color: '#374151'
  }
});

export default Remote;
