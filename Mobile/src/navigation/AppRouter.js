import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { NavigationContainer } from '@react-navigation/native';
import { Ionicons } from '@expo/vector-icons';

// Ekranları import et
import Remote from '../screens/Remote';
import Connect from '../screens/Connect';
import Login from '../screens/Login';
import Therapy from '../screens/Therapy';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

/**
 * Ana uygulama ekranlarını (Kontrol ve Terapi) içeren alt sekme gezgini (Bottom Tab Navigator).
 * Kullanıcı giriş yaptıktan sonra bu ekranlar arasında geçiş yapabilir.
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
        tabBarActiveTintColor: '#5D3FD3', // Aktif sekme rengi
        tabBarInactiveTintColor: 'gray', // Pasif sekme rengi
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
 * Bağlantı, giriş ve ana uygulama ekranları arasındaki geçişleri yönetir.
 */
const AppRouter = () => {
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
