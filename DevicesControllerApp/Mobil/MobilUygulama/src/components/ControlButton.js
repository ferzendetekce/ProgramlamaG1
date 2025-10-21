import React from 'react';
import { TouchableOpacity, Text, View } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

/**
 * Uygulama genelinde kullanılan, ikon ve metin içerebilen genel amaçlı bir kontrol butonu.
 * @param {object} props - Komponentin özellikleri.
 * @param {function} props.onPress - Butona basıldığında çalışacak fonksiyon.
 * @param {string} [props.icon] - Butonda gösterilecek Ionicons ikonunun adı.
 * @param {string} [props.text] - Butonda gösterilecek metin.
 * @param {string} props.colorClass - Butonun arka plan rengini belirleyen Tailwind CSS sınıfı.
 * @param {string} [props.textClass='text-white'] - Buton metninin rengini belirleyen Tailwind CSS sınıfı.
 * @param {'default'|'large'|'small'} [props.size='default'] - Butonun boyutunu belirler.
 */
const ControlButton = ({ onPress, icon, text, colorClass, textClass = 'text-white', size = 'default' }) => {
    // Buton boyutlarına göre stilleri belirle
    const sizeClasses = {
        default: 'px-6 py-4 rounded-2xl flex-row items-center',
        large: 'p-7 rounded-3xl',
        small: 'p-3 rounded-xl'
    };
    // İkon boyutlarını belirle
    const iconSize = {
        default: 24,
        large: 36,
        small: 20
    }

    return (
        <TouchableOpacity
            className={`${sizeClasses[size]} ${colorClass} shadow-lg active:scale-95 items-center justify-center m-2`}
            onPress={onPress}
        >
            <View className="flex-row items-center">
                {icon && <Ionicons name={icon} size={iconSize[size]} color={textClass.includes('white') ? 'white' : 'black'} />}
                {text && icon && <View className="w-2"/> /* İkon ve metin arasında boşluk */}
                {text && <Text className={`font-bold text-lg ${textClass}`}>{text}</Text>}
            </View>
        </TouchableOpacity>
    );
};

export default ControlButton;
