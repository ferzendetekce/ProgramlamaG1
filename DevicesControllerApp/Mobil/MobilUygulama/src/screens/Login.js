import { View, Text, ActivityIndicator, StyleSheet } from 'react-native';
import React, { useState, useEffect } from 'react';
import AsyncStorage from "@react-native-async-storage/async-storage";
import PInput from '../components/PInput';
import PButton from '../components/PButton';
import statusService from '../services/statusService';

import authService from '../services/authService';

const Login = ({ navigation }) => {
  const [username, setUsername] = useState("grup11");
  const [password, setPassword] = useState("12345");
  const [host, setHost] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const loadHost = async () => {
      try {
        const storedHost = await AsyncStorage.getItem("apiHost");
        if (storedHost) {
          const parsedHost = JSON.parse(storedHost);
          setHost(parsedHost);
          console.log("Login Ekranı: Bağlantı bilgisi yüklendi:", parsedHost);
        } else {
          navigation.navigate("Connect");
        }
      } catch (err) {
        setError("Bağlantı ayarları okunamadı.");
        navigation.navigate("Connect");
      }
    };
    loadHost();
  }, [navigation]);

  const handleLogin = async () => {
    if (!host || !host.ip || !host.port) {
      setError("Bağlantı bilgileri eksik. Lütfen geri dönüp tekrar deneyin.");
      return;
    }

    setLoading(true);
    setError('');

    try {
      const token = await authService.login(host.ip, host.port, username, password);
      if (token && typeof token === 'string') {
        await AsyncStorage.setItem("authToken", token);
        navigation.replace('MainTabs');
      } else {
        setError("Giriş yanıtı anlaşılamadı.");
      }
    } catch (err) {
      setError(err.message || "Giriş sırasında bilinmeyen bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  const handleBackDisconnect = async () => {
    try {
      await statusService.sendDisconnect();
    } catch {}
    await AsyncStorage.removeItem("authToken");
    navigation.navigate("Connect");
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Kullanıcı Girişi</Text>
      <View style={styles.inputContainer}>
        <PButton onPress={handleBackDisconnect} className="mb-4 bg-gray-400">
          Geri / Bağlantıyı Kes
        </PButton>

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
