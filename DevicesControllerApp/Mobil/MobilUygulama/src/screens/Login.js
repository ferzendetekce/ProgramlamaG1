
import { View, Text, ActivityIndicator, StyleSheet } from 'react-native';
import React, { useState, useEffect } from 'react';
import AsyncStorage from "@react-native-async-storage/async-storage";
import PInput from '../components/PInput';
import PButton from '../components/PButton';

import authService from '../services/authService';

const Login = ({ navigation }) => {
  const [username, setUsername] = useState("grup11"); // Varsayılan kullanıcı adı
  const [password, setPassword] = useState("12345"); // Varsayılan şifre
  const [host, setHost] = useState(null); // Backend IP ve Port bilgisi {ip: '...', port: '...'}
  const [error, setError] = useState(''); // Ekranda gösterilecek hata mesajı
  const [loading, setLoading] = useState(false); // Giriş işlemi sırasında yükleme göstergesi

  useEffect(() => {
    const loadHost = async () => {
      try {
        const storedHost = await AsyncStorage.getItem("apiHost");
        if (storedHost) {
          const parsedHost = JSON.parse(storedHost);
          setHost(parsedHost);
          console.log("Login Ekranı: Bağlantı bilgisi yüklendi:", parsedHost);
        } else {
          console.warn("Login Ekranı: Bağlantı bilgisi bulunamadı. Connect ekranına yönlendiriliyor.");
          navigation.navigate("Connect");
        }
      } catch (err) {
        console.error("Login Ekranı: Bağlantı bilgisi okunurken hata:", err);
        setError("Bağlantı ayarları okunamadı.");
        navigation.navigate("Connect");
      }
    };
    loadHost();
  }, [navigation]); 
  const handleLogin = async () => {
    if (!host || !host.ip || !host.port) {
      setError("Bağlantı bilgileri eksik veya yüklenemedi. Lütfen geri dönüp tekrar deneyin.");
      console.error("handleLogin: Host bilgisi eksik:", host);
      return;
    }

    setLoading(true); // Yükleme göstergesini başlat
    setError(''); // Önceki hataları temizle

    try {
      console.log(`Giriş deneniyor: ${host.ip}:${host.port}, Kullanıcı: ${username}`);
      
      const token = await authService.login(host.ip, host.port, username, password);
      if (token && typeof token === 'string') {
        console.log("Giriş başarılı, token alındı.");

        try {
          await AsyncStorage.setItem("authToken", token);
          console.log("Token başarıyla AsyncStorage'a kaydedildi.");

          navigation.replace('MainTabs');

        } catch (storageError) {
            console.error("handleLogin: Token kaydedilirken hata:", storageError);
            setError("Oturum bilgisi kaydedilemedi. Tekrar deneyin.");
        }

      } else {
         console.error("handleLogin: AuthService beklenen token'ı döndürmedi.");
         setError("Giriş yanıtı anlaşılamadı.");
      }
    } catch (err) {
      console.error("handleLogin: Giriş işlemi sırasında hata yakalandı:", err);
      setError(err.message || "Giriş sırasında bilinmeyen bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Kullanıcı Girişi</Text>
      <View style={styles.inputContainer}>
        <PInput
          value={username}
          onChangeText={setUsername}
          placeholder='Kullanıcı Adı'
          className="mb-4" 
          autoCapitalize="none" 
          editable={!loading} 
        />
        <PInput
          value={password}
          onChangeText={setPassword}
          placeholder='Şifre'
          secureTextEntry 
          className="mb-6" 
          editable={!loading} 
        />

        {error ? (
          <Text style={styles.errorText}>{error}</Text>
        ) : null}

        <PButton onPress={handleLogin} disabled={loading}>
          {loading ? (
            <ActivityIndicator color="white" /> 
          ) : (
            'Giriş Yap' 
          )}
        </PButton>
      </View>
    </View>
  );
}
const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#E2E8F0', 
    padding: 20, 
  },
  title: {
    fontSize: 30, 
    fontWeight: 'bold',
    color: '#1F2937', 
    marginBottom: 32,
    textAlign: 'center',
  },
  inputContainer: {
    width: '100%',
    maxWidth: 384, 
  },
  errorText: {
    color: '#EF4444', 
    textAlign: 'center',
    marginBottom: 16, 
    fontWeight: '600', 
  },
});

export default Login;