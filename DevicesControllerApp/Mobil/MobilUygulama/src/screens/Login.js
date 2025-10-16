import { View, Text, ActivityIndicator } from 'react-native';
import React, { useState, useEffect } from 'react';
import AsyncStorage from "@react-native-async-storage/async-storage";
import PInput from '../components/PInput';
import PButton from '../components/PButton';
import authService from '../services/authService';

/**
 * Kullanıcı adı ve şifre ile giriş yapılan ekran.
 */
const Login = ({ navigation }) => {
  const [username, setUsername] = useState("grup11");
  const [password, setPassword] = useState("12345");
  const [host, setHost] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  // Ekran yüklendiğinde hafızadan bağlantı (host) bilgilerini çeker.
  useEffect(() => {
    const loadHost = async () => {
      const value = await AsyncStorage.getItem("apiHost");
      if (value) {
        setHost(JSON.parse(value));
      } else {
        // Host bilgisi bulunamazsa kullanıcıyı tekrar bağlantı ekranına yönlendir.
        navigation.navigate("Connect");
      }
    };
    loadHost();
  }, []);

  /**
   * Giriş yap butonuna basıldığında çalışır.
   * `authService` üzerinden backend'e giriş isteği gönderir.
   */
  const handleLogin = async () => {
    if (!host) {
      setError("Bağlantı bilgileri bulunamadı. Lütfen geri dönüp tekrar deneyin.");
      return;
    }
    setLoading(true);
    setError('');
    try {
      const data = await authService.login(host.ip, host.port, username, password);
      if (data.status === 'ok' && data.token) {
        // Başarılı girişte alınan token'ı kaydet ve ana ekrana yönlendir.
        await AsyncStorage.setItem("authToken", data.token);
        navigation.replace('MainTabs'); // 'replace' kullanılarak Login ekranına geri dönülemez.
      }
    } catch (err) {
      setError(err.message || "Giriş sırasında bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <View className='flex-1 items-center justify-center bg-slate-200 p-5'>
      <Text className="text-3xl font-bold text-gray-800 mb-8">Kullanıcı Girişi</Text>
      <View className='w-full max-w-sm'>
        <PInput
          value={username}
          onChangeText={setUsername}
          placeholder='Kullanıcı Adı'
          className="mb-4"
          autoCapitalize="none"
        />
        <PInput
          value={password}
          onChangeText={setPassword}
          placeholder='Şifre'
          secureTextEntry
          className="mb-6"
        />
        {error ? <Text className="text-red-500 text-center mb-4 font-semibold">{error}</Text> : null}
        <PButton onPress={handleLogin} disabled={loading}>
          {loading ? <ActivityIndicator color="white" /> : 'Giriş Yap'}
        </PButton>
      </View>
    </View>
  );
}

export default Login;
