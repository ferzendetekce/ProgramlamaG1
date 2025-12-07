import { View, Text, ActivityIndicator, Alert, StyleSheet, Animated, Easing } from 'react-native';
import AsyncStorage from "@react-native-async-storage/async-storage";
import React, { useEffect, useRef, useState } from 'react';
import PInput from '../components/PInput';
import PButton from '../components/PButton';
import statusService from '../services/statusService';

const Connect = ({ navigation }) => {
  const [ip, setIp] = useState("10.200.117.50");
  const [port, setPort] = useState("5086");
  const [checking, setChecking] = useState(false);
  const [error, setError] = useState('');
  const [debugVisible, setDebugVisible] = useState(false);
  const [debugInfo, setDebugInfo] = useState('');
  const [debugError, setDebugError] = useState('');
  const fadeAnim = useRef(new Animated.Value(0)).current;

  useEffect(() => {
    Animated.timing(fadeAnim, {
      toValue: 1,
      duration: 500,
      easing: Easing.out(Easing.ease),
      useNativeDriver: true,
    }).start();
  }, [fadeAnim]);

  const handleConnect = async () => {
    setError('');
    setChecking(true);
    try {
      await statusService.checkConnection(ip, port);
      await AsyncStorage.setItem("apiHost", JSON.stringify({ ip, port }));
      navigation.navigate("Login");
    } catch (err) {
      console.error("Bağlantı doğrulanamadı:", err);
      setError(err.message || "Sunucuya ulaşılamadı.");
      Alert.alert("Bağlantı Hatası", err.message || "Sunucuya ulaşılamadı.");
    } finally {
      setChecking(false);
    }
  };

  const handleDebugPing = async () => {
    setDebugError('');
    setDebugInfo('Ping atılıyor...');
    try {
      const res = await statusService.checkConnection(ip, port, 1000);
      setDebugInfo(`Ping OK -> reachable: ${res.reachable ? "true" : "false"}`);
    } catch (err) {
      setDebugError(err.message || "Ping başarısız.");
      setDebugInfo('');
    }
  };

  const handleAutoDiscover = async () => {
    setError('');
    setChecking(true);
    try {
      const found = await statusService.autoDiscover();
      setIp(found.ip);
      setPort(String(found.port));
      await AsyncStorage.setItem("apiHost", JSON.stringify(found));
      Alert.alert("Bulundu", `Sunucu: ${found.ip}:${found.port}`);
      navigation.navigate("Login");
    } catch (err) {
      console.error("Otomatik keşif başarısız:", err);
      setError(err.message || "Sunucu bulunamadı.");
    } finally {
      setChecking(false);
    }
  };

  return (
    <Animated.View style={[styles.container, { opacity: fadeAnim, transform: [{ translateY: fadeAnim.interpolate({ inputRange: [0,1], outputRange: [20,0] }) }] }]}>
      <Text style={styles.title}>Cihaza Bağlan</Text>
      <View style={styles.card}>
        <PInput
          value={ip}
          onChangeText={setIp}
          placeholder='IP Adresi'
          className="mb-4"
        />
        <PInput
          value={port}
          onChangeText={setPort}
          placeholder='Port Numarası'
          keyboardType='numeric'
          className="mb-6"
        />
        {error ? <Text style={styles.error}>{error}</Text> : null}
        <PButton onPress={handleConnect} disabled={checking}>
          {checking ? <ActivityIndicator color="white" /> : "İlerle"}
        </PButton>
        <View style={{ height: 12 }} />
        <PButton onPress={handleAutoDiscover} disabled={checking}>
          {checking ? <ActivityIndicator color="white" /> : "Otomatik Bul"}
        </PButton>
        <View style={{ height: 10 }} />
        <PButton onPress={() => setDebugVisible(!debugVisible)} disabled={checking} className="bg-slate-500">
          {debugVisible ? "Debug Gizle" : "Debug Göster"}
        </PButton>
        {debugVisible ? (
          <View style={styles.debugBox}>
            <PButton onPress={handleDebugPing} disabled={checking} className="bg-cyan-600 mb-2">
              {checking ? <ActivityIndicator color="white" /> : "Ping Testi"}
            </PButton>
            {debugInfo ? <Text style={styles.debugInfo}>{debugInfo}</Text> : null}
            {debugError ? <Text style={styles.debugError}>{debugError}</Text> : null}
          </View>
        ) : null}
      </View>
    </Animated.View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#E8ECF4',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20
  },
  title: {
    fontSize: 28,
    fontWeight: '800',
    color: '#0F172A',
    marginBottom: 16
  },
  card: {
    width: '100%',
    maxWidth: 420,
    backgroundColor: '#FFFFFF',
    padding: 18,
    borderRadius: 12,
    shadowColor: '#000',
    shadowOpacity: 0.08,
    shadowOffset: { width: 0, height: 4 },
    shadowRadius: 12,
    elevation: 4
  },
  error: {
    textAlign: 'center',
    color: '#DC2626',
    marginBottom: 12,
    fontWeight: '600'
  },
  debugBox: {
    marginTop: 10,
    padding: 10,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#E5E7EB',
    backgroundColor: '#F8FAFC'
  },
  debugInfo: {
    color: '#065F46',
    fontWeight: '700'
  },
  debugError: {
    color: '#B91C1C',
    fontWeight: '700'
  }
});

export default Connect;
