import * as Notifications from 'expo-notifications';
import * as Device from 'expo-device';
import { Platform, Alert } from 'react-native';

// Uygulama ön plandayken bildirimlerin gösterilmesini sağlar.
Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: false,
  }),
});

/**
 * Kullanıcıdan anlık bildirim göndermek için izin ister.
 * @returns {Promise<boolean>} İzin verilip verilmediğini belirtir.
 */
export async function registerForPushNotificationsAsync() {
  if (!Device.isDevice) {
    console.warn('Bildirim özellikleri sadece fiziksel cihazlarda test edilebilir.');
    return false;
  }

  const { status: existingStatus } = await Notifications.getPermissionsAsync();
  let finalStatus = existingStatus;
  
  if (existingStatus !== 'granted') {
    const { status } = await Notifications.requestPermissionsAsync();
    finalStatus = status;
  }
  
  if (finalStatus !== 'granted') {
    Alert.alert(
      'İzin Gerekli', 
      'Uygulamanın bildirim gönderebilmesi için ayarlardan izin vermeniz gerekmektedir.'
    );
    return false;
  }

  if (Platform.OS === 'android') {
    await Notifications.setNotificationChannelAsync('default', {
      name: 'default',
      importance: Notifications.AndroidImportance.MAX,
      vibrationPattern: [0, 250, 250, 250],
      lightColor: '#FF231F7C',
    });
  }
  
  return true;
}

// --- YENİ FONKSİYON ---
/**
 * Belirtilen başlık ve metin ile anında bir bildirim gönderir.
 * @param {string} title - Bildirimin başlığı.
 * @param {string} body - Bildirimin içeriği.
 */
export async function sendImmediateNotification(title, body) {
  console.log(`Bildirim gönderiliyor: "${title}" - "${body}"`);
  await Notifications.scheduleNotificationAsync({
    content: {
      title: title || "Rehabilitasyon Cihazı",
      body: body,
    },
    trigger: null, // Anında gönder
  });
}


/**
 * Hata oluştuğunda bir bildirim gönderir.
 * @param {string} errorMessage - Gösterilecek özel hata mesajı.
 */
export async function sendErrorNotification(errorMessage) {
  await sendImmediateNotification("⚠️ Terapi Hatası", errorMessage || 'Cihazla iletişimde bir sorun oluştu.');
}

/**
 * Görünen ve zamanlanmış tüm bildirimleri temizler.
 */
export async function cancelAllNotifications() {
  console.log("Tüm bildirimler temizleniyor...");
  await Notifications.dismissAllNotificationsAsync();
  await Notifications.cancelAllScheduledNotificationsAsync();
}

