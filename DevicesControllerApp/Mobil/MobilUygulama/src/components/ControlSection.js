import React from 'react';
import { View, Text } from 'react-native';

/**
 * Kontrol gruplarını sarmalayan ve başlık ekleyen bir bileşen.
 * @param {object} props - Komponentin özellikleri.
 * @param {string} props.title - Bölüm başlığı.
 * @param {React.ReactNode} props.children - Bölümün içeriği.
 */
const ControlSection = ({ title, children }) => (
  <View className="w-full max-w-md items-center bg-white rounded-3xl p-6 shadow-lg mb-6">
    <Text className="text-gray-700 text-xl font-bold mb-5">{title}</Text>
    {children}
  </View>
);

export default ControlSection;
