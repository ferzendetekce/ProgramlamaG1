import { TouchableOpacity } from 'react-native'
import React from 'react'
import { Ionicons } from '@expo/vector-icons';

const Up = ({onPress}) => {
  return (
    <TouchableOpacity className="p-7 bg-indigo-500 rounded-3xl mb-4 shadow-xl active:bg-indigo-600 active:scale-95"
      onPress={onPress}
    >
      <Ionicons name="arrow-up" size={36} color="white" />
    </TouchableOpacity>
  )
}

export default Up;