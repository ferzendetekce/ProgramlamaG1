import { View, Text} from 'react-native'
import AsyncStorage from "@react-native-async-storage/async-storage";
import React, { useState } from 'react'
import PInput from '../components/PInput';
import PButton from '../components/PButton';

/**
 * Kullanıcının backend servisinin IP ve Port bilgilerini girdiği ekran.
 */
const Connect = ({ navigation }) => {
  const [ip, setIp] = useState("172.20.10.2");
  const [port, setPort] = useState("5086");

  const handleConnect = async () => {
    await AsyncStorage.setItem("apiHost", JSON.stringify( {ip, port} ));
    navigation.navigate("Login");
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
        <PButton onPress={handleConnect}>İlerle</PButton>
      </View>
    </View>
  )
}

export default Connect;
