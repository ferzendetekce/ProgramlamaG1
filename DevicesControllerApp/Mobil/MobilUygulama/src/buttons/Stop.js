import { TouchableOpacity, Text } from 'react-native';
import React from 'react';

const Stop = ({onPress}) => (
  <TouchableOpacity className="px-10 py-5 bg-rose-500 rounded-3xl shadow-xl active:bg-rose-600"
    onPress={onPress}
  >
    <Text className="text-white font-bold text-xl">STOP</Text>
  </TouchableOpacity>
);

export default Stop;