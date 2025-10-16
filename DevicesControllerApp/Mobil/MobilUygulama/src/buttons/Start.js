import { TouchableOpacity, Text } from 'react-native';
import React from 'react';

const Start = ({ onPress }) => (
  <TouchableOpacity className="px-10 py-5 bg-emerald-500 rounded-3xl mr-4 shadow-xl active:bg-emerald-600"
    onPress={onPress}
  >
    <Text className="text-white font-bold text-xl">START</Text>
  </TouchableOpacity>
);

export default Start;