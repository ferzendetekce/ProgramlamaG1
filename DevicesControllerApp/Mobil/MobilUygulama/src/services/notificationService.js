// src/services/notificationService.js

import * as Notifications from 'expo-notifications';
import * as Device from 'expo-device';
import { Platform } from 'react-native';

// Uygulama açıkken bildirimlerin nasıl davranacağını ayarlar.
Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: false,
  }),
});

// Bildirim göndermek için izin isteyen fonksiyon
export async function registerForPushNotificationsAsync() {
  if (Device.isDevice) {
    const { status: existingStatus } = await Notifications.getPermissionsAsync();
    let finalStatus = existingStatus;
    if (existingStatus !== 'granted') {
      const { status } = await Notifications.requestPermissionsAsync();
      finalStatus = status;
    }
    if (finalStatus !== 'granted') {
      alert('Bildirim almak için izin vermeniz gerekiyor!');
      return;
    }
  } else {
    alert('Bildirimler için fiziksel bir cihaz kullanılmalıdır.');
  }

  if (Platform.OS === 'android') {
    Notifications.setNotificationChannelAsync('default', {
      name: 'default',
      importance: Notifications.AndroidImportance.MAX,
      vibrationPattern: [0, 250, 250, 250],
      lightColor: '#FF231F7C',
    });
  }
}

// "Terapi Devam Ediyor" bildirimini gönderir
export async function scheduleTherapyInProgressNotification() {
  await Notifications.scheduleNotificationAsync({
    content: {
      title: "Terapi Devam Ediyor",
      body: 'Terapi seansınız şu anda aktif.',
      sticky: true, // Android'de bildirimin kaydırılarak kapatılmasını engeller
    },
    trigger: null, // null anında gönderir
  });
}

// "Hata Oluştu" bildirimini anında gönderir
export async function sendErrorNotification(errorMessage) {
  await Notifications.scheduleNotificationAsync({
    content: {
      title: "⚠️ Terapi Hatası",
      body: errorMessage || 'Cihazla iletişimde bir sorun oluştu.',
    },
    trigger: null,
  });
}

// Tüm zamanlanmış veya aktif bildirimleri iptal eder
export async function cancelAllNotifications() {
  await Notifications.dismissAllNotificationsAsync(); // Görünenleri kapat
  await Notifications.cancelAllScheduledNotificationsAsync(); // Zamanlanmış olanları iptal et
}