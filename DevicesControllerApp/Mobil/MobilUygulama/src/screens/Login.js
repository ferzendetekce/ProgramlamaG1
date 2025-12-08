import { View, Text, ActivityIndicator, StyleSheet } from 'react-native';
import React, { useState, useEffect } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import PInput from '../components/PInput';
import PButton from '../components/PButton';
import authService from '../services/authService';

const Login = ({ navigation }) => {
  const [username, setUsername] = useState('grup11');
  const [password, setPassword] = useState('12345');
  const [host, setHost] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const loadHost = async () => {
      try {
        const storedHost = await AsyncStorage.getItem('apiHost');
        if (storedHost) {
          const parsedHost = JSON.parse(storedHost);
          setHost(parsedHost);
          console.log('Login Ekrani: Baglanti bilgisi yüklendi:', parsedHost);
        } else {
          navigation.navigate('Connect');
        }
      } catch (err) {
        setError('Baglanti ayarlari okunamadi.');
        navigation.navigate('Connect');
      }
    };
    loadHost();
  }, [navigation]);

  const handleLogin = async () => {
    if (!host || !host.ip || !host.port) {
      setError('Baglanti bilgileri eksik. Lutfen geri donup tekrar deneyin.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const token = await authService.login(host.ip, host.port, username, password);
      if (token && typeof token === 'string') {
        await AsyncStorage.setItem('authToken', token);
        navigation.replace('MainTabs');
      } else {
        setError('Giris yaniti anlasilamadi.');
      }
    } catch (err) {
      setError(err.message || 'Giris sirasinda bilinmeyen bir hata olustu.');
    } finally {
      setLoading(false);
    }
  };

  const handleBack = async () => {
    await AsyncStorage.removeItem('authToken');
    navigation.navigate('Connect');
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Kullanici Girisi</Text>
      <View style={styles.inputContainer}>
        <PButton onPress={handleBack} className="mb-4 bg-gray-400">
          Geri
        </PButton>

        <PInput
          value={username}
          onChangeText={setUsername}
          placeholder="Kullanici Adi"
          className="mb-4"
          autoCapitalize="none"
          editable={!loading}
        />
        <PInput
          value={password}
          onChangeText={setPassword}
          placeholder="Sifre"
          secureTextEntry
          className="mb-6"
          editable={!loading}
        />

        {error ? <Text style={styles.errorText}>{error}</Text> : null}

        <PButton onPress={handleLogin} disabled={loading}>
          {loading ? <ActivityIndicator color="white" /> : 'Giris Yap'}
        </PButton>
      </View>
    </View>
  );
};

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
