import React, { useEffect } from 'react'; // useEffect'i import ettiğimizden emin oluyoruz
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { NavigationContainer } from '@react-navigation/native';
import { Ionicons } from '@expo/vector-icons';

// Yeni notificationService'imizi import ediyoruz
import { registerForPushNotificationsAsync } from '../services/notificationService';

// Ekranları import ediyoruz
import Remote from '../screens/Remote';
import Connect from '../screens/Connect';
import Login from '../screens/Login';
import Therapy from '../screens/Therapy';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

/**
 * Ana uygulama ekranlarını (Kontrol ve Terapi) içeren alt sekme gezgini (Bottom Tab Navigator).
 * Bu fonksiyonun içeriğini eksik vermiştim, doğrusu bu.
 */
const MainTabs = () => {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        headerShown: false,
        tabBarIcon: ({ focused, color, size }) => {
          let iconName;
          if (route.name === 'RemoteControl') {
            iconName = focused ? 'game-controller' : 'game-controller-outline';
          } else if (route.name === 'TherapyInfo') {
            iconName = focused ? 'body' : 'body-outline';
          }
          return <Ionicons name={iconName} size={size} color={color} />;
        },
        tabBarActiveTintColor: '#5D3FD3',
        tabBarInactiveTintColor: 'gray',
        tabBarStyle: {
            backgroundColor: '#FFFFFF',
            borderTopWidth: 0,
            elevation: 10,
        }
      })}
    >
      <Tab.Screen name="RemoteControl" component={Remote} options={{ title: 'Kontrol' }} />
      <Tab.Screen name="TherapyInfo" component={Therapy} options={{ title: 'Terapi' }} />
    </Tab.Navigator>
  );
};

/**
 * Uygulamanın ana yönlendiricisi (Router).
 * Şimdi bildirim iznini isteyecek kodu ekliyoruz.
 */
const AppRouter = () => {
  // Bu useEffect bloğu, uygulama açılır açılmaz çalışır.
  useEffect(() => {
    // Bildirim gönderebilmek için kullanıcıdan izin istiyoruz.
    registerForPushNotificationsAsync();
  }, []);

  return (
    <NavigationContainer>
      <Stack.Navigator initialRouteName='Connect' screenOptions={{ headerShown: false }}>
        <Stack.Screen name='Connect' component={Connect} />
        <Stack.Screen name='Login' component={Login} />
        <Stack.Screen name='MainTabs' component={MainTabs} />
      </Stack.Navigator>
    </NavigationContainer>
  )
}
export default AppRouter;