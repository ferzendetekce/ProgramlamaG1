import { View, Text, ActivityIndicator, Alert } from 'react-native'
import AsyncStorage from "@react-native-async-storage/async-storage";
import React, { useState } from 'react'
import PInput from '../components/PInput';
import PButton from '../components/PButton';
import statusService from '../services/statusService';

/**
 * Kullanıcının backend servisinin IP ve Port bilgilerini girdiği ekran.
 */
const Connect = ({ navigation }) => {
  const [ip, setIp] = useState("172.20.10.2");
  const [port, setPort] = useState("5086");
  const [checking, setChecking] = useState(false);
  const [error, setError] = useState('');

  const handleConnect = async () => {
    setError('');
    setChecking(true);
    try {
      await statusService.checkConnection(ip, port);
      await AsyncStorage.setItem("apiHost", JSON.stringify( {ip, port} ));
      navigation.navigate("Login");
    } catch (err) {
      console.error("Bağlantı doğrulanamadı:", err);
      setError(err.message || "Sunucuya ulaşılamadı.");
      Alert.alert("Bağlantı Hatası", err.message || "Sunucuya ulaşılamadı.");
    } finally {
      setChecking(false);
    }
  }

  return (
    <View className='flex-1 items-center justify-center bg-slate-200 p-5'>
      <Text className="text-3xl font-bold text-gray-800 mb-8 text-center">Cihaza Bağlan</Text>
      <View className='w-full max-w-sm'>
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
        {error ? <Text className="text-center text-red-600 mb-4">{error}</Text> : null}
        <PButton onPress={handleConnect} disabled={checking}>
          {checking ? <ActivityIndicator color="white" /> : "İlerle"}
        </PButton>
      </View>
    </View>
  )
}

export default Connect;
