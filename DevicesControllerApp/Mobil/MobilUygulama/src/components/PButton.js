import { StyleSheet, Text, TouchableOpacity, View } from 'react-native'
import React from 'react'

const PButton = ({ children, onPress }) => {
  return (
    <TouchableOpacity className='bg-[#5D3FD3] rounded-xl h-11 items-center justify-center' activeOpacity={0.7} onPress={onPress}>
      <Text className='text-white text-lg'>{children}</Text>
    </TouchableOpacity>
  )
}

export default PButton;