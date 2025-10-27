# Proje Gereksinimleri

Bu proje iki ana bölümden oluşmaktadır: Backend (.NET) ve Frontend (React Native).

## Backend Gereksinimleri (.NET)

Backend'in çalışması için **.NET 9.0** yüklü olmalıdır. Proje bağımlılıkları `EngineAPI/EngineAPI.csproj` dosyasında belirtilmiştir:

* **Framework**: `net9.0`
* **Paketler**:
    * `Microsoft.AspNetCore.OpenApi` (Versiyon 9.0.3)

Bu bağımlılıklar, `dotnet restore` komutu çalıştırıldığında otomatik olarak indirilecektir.

## Frontend Gereksinimleri (Node.js - npm)

Frontend'in çalışması için **Node.js** ve **npm** (veya yarn) yüklü olmalıdır. Tüm bağımlılıklar `package.json` dosyasında listelenmiştir ve `npm install` komutu ile yüklenebilir.

### Ana Bağımlılıklar (`dependencies`):

* `@react-native-async-storage/async-storage`: ^2.2.0
* `@react-navigation/bottom-tabs`: ^7.4.7
* `@react-navigation/native`: ^7.1.17
* `@react-navigation/native-stack`: ^7.3.26
* `@rn-primitives/slot`: ^1.2.0
* `expo`: ~54.0.10
* `expo-status-bar`: ~3.0.8
* `nativewind`: ^4.2.1
* `react`: 19.1.0
* `react-native`: 0.81.4
* `react-native-safe-area-context`: ^5.6.1
* `react-native-screens`: ^4.16.0
* `react-native-vector-icons`: ^10.3.0

### Geliştirme Bağımlılıkları (`devDependencies`):

* `@types/react`: ~19.1.10
* `babel-preset-expo`: ^54.0.3
* `tailwindcss`: ^3.3.2
* `typescript`: ~5.9.2