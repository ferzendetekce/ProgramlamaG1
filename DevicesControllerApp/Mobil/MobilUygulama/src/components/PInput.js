import { StyleSheet, Text, TextInput, View } from 'react-native'
import React from 'react'

const PInput = ({ className, value, onChangeText, placeholder, keyboardType, ...props }) => {
  return (
    <View className='m-2'>
      <TextInput
        className={`rounded-lg bg-gray-400 p-3 ${className}`}
        value={value}
        onChangeText={onChangeText}
        placeholder={placeholder}
        keyboardType={keyboardType}
        {...props}
      />
    </View>
  )
}

export default PInput;