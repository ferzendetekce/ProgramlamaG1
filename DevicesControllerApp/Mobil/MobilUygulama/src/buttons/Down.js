import { TouchableOpacity } from 'react-native';
import React from 'react';
import { Ionicons } from '@expo/vector-icons';

const Down = ({ onPress }) => (
  <TouchableOpacity className="p-7 bg-indigo-500 rounded-3xl mt-4 shadow-xl active:bg-indigo-600"
    onPress={onPress}
  >
    <Ionicons name="arrow-down" size={36} color="white" />
  </TouchableOpacity>
);

export default Down;