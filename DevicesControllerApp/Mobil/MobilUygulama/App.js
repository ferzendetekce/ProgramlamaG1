import "./global.css";
import AppRouter from './src/navigation/AppRouter';
import React, { useEffect } from 'react';

// Bildirim servisinden izin isteme fonksiyonunu import et
import { registerForPushNotificationsAsync } from './src/services/notificationService';

export default function App() {
  // useEffect hook'u, uygulama ilk yüklendiğinde bir kez çalışır.
  useEffect(() => {
    // Kullanıcıdan bildirim göndermek için izin iste.
    // Bu fonksiyon, notificationService.js içinde tanımlanmıştır.
    registerForPushNotificationsAsync()
      .then(() => {
        console.log("Bildirim izni kontrolü tamamlandı.");
      })
      .catch(error => {
        console.error("Bildirim izni istenirken bir hata oluştu:", error);
      });
  }, []); // Boş bağımlılık dizisi, bu etkinin sadece bir kez çalışmasını sağlar.

  return <AppRouter />;
}
