import { TouchableOpacity } from 'react-native'
import React from 'react'
import { Ionicons } from '@expo/vector-icons';

const Left = ({ onPress }) => {
  return (
    <TouchableOpacity className="p-7 bg-indigo-500 rounded-3xl mr-4 shadow-xl active:bg-indigo-600 active:scale-95"
      onPress={onPress}
    >
      <Ionicons name="arrow-back" size={36} color="white" />
    </TouchableOpacity>
  )
}

export default Left;