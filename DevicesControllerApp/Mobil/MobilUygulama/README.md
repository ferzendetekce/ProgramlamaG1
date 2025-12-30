
# Rehabilitasyon Sistemi - Mobil Uygulama

Bu proje, rehabilitasyon cihazlarının uzaktan kontrolü için geliştirilmiş bir mobil uygulama ve API altyapısıdır.

## Sistem Mimarisi

```
Mobil Uygulama (React Native + Expo)
         ↓ HTTP/REST API
    EngineAPI (ASP.NET Core 9.0 - Port 5000)
         ↓ TCP Socket (Port 9000)
    MobileCommandServer (Windows Forms App)
         ↓ Serial Communication
    DeviceCommunication
         ↓ RS-232/USB
    Fiziksel Rehabilitasyon Cihazı
```

### Katmanlar

1. **Mobil Uygulama**: React Native ile geliştirilmiş, terapi kontrolü ve izleme arayüzü
2. **EngineAPI**: HTTP isteklerini TCP komutlarına çeviren köprü servisi
3. **MobileCommandServer**: Windows ana formu üzerinde çalışan TCP sunucusu
4. **DeviceCommunication**: Fiziksel cihazla seri port üzerinden iletişim

## Projeyi Çalıştırma

Sistemin tam çalışması için 3 bileşenin sırasıyla başlatılması gerekir:

### 1. Ana Windows Uygulaması

Ana rehabilitasyon sistemi uygulaması çalışıyor olmalıdır. Bu uygulama:
- `DeviceCommunication` üzerinden cihazla iletişim kurar
- `MobileCommandServer` ile Port 9000'de TCP dinleme yapar

### 2. Backend - EngineAPI (.NET Web API)

Backend, `EngineAPI` klasöründe yer alan bir .NET projesidir.

**Gereksinimler:**
* [.NET SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

**Kurulum ve Çalıştırma:**

1.  **Bağımlılıkları Yükleme:**
    ```bash
    cd EngineAPI
    dotnet restore
    ```

2.  **Projeyi Derleme:**
    ```bash
    dotnet build
    ```

3.  **API'yi Başlatma:**
    ```bash
    dotnet run
    ```

    API varsayılan olarak `http://localhost:5000` portunda çalışmaya başlayacaktır.

**API Endpoints:**

- `POST /api/command/start` - Terapiyi başlat
- `POST /api/command/stop` - Terapiyi durdur
- `POST /api/command/pause` - Terapiyi beklet
- `POST /api/command/resume` - Terapiye devam et
- `POST /api/command/emergency` - Acil durdurma
- `POST /api/command/up`, `down`, `left`, `right` - Hareket kontrolleri
- `POST /api/command/footincrease`, `footdecrease` - Ayak numarası ayarı
- `POST /api/command/barup`, `bardown` - Destek barı kontrolü
- `POST /api/command/weightincrease`, `weightdecrease` - Ağırlık ayarı
- `GET /api/status` - Mevcut terapi durumu
- `POST /api/auth/login` - Kullanıcı girişi

### 3. Frontend (React Native - Expo)

Frontend, projenin ana dizininde bulunan bir React Native projesidir ve Expo kullanılarak geliştirilmiştir.

**Gereksinimler:**
* [Node.js](https://nodejs.org/) (LTS versiyonu önerilir)
* [Expo Go](https://expo.dev/client) uygulaması (mobil cihazınızda test etmek için)

**Kurulum ve Çalıştırma:**

1.  **Bağımlılıkları Yükleme:**
    ```bash
    npm install
    # veya
    yarn install
    ```

2.  **Uygulamayı Başlatma:**
    
    * Geliştirme sunucusunu başlatmak için:
        ```bash
        npm start
        ```
    * Doğrudan Android emülatöründe çalıştırmak için:
        ```bash
        npm run android
        ```
    * Doğrudan iOS simülatöründe çalıştırmak için (sadece macOS):
        ```bash
        npm run ios
        ```

    `npm start` komutunu çalıştırdıktan sonra terminalde bir QR kod göreceksiniz. Bu QR kodu mobil cihazınızdaki **Expo Go** uygulaması ile okutarak projeyi telefonunuzda canlı olarak test edebilirsiniz.

## Uygulama Akışı

1. **Bağlantı Ekranı**: Backend API'sinin çalıştığı bilgisayarın yerel ağdaki IP adresini ve portunu (5000) girin.
   - Örnek: `192.168.1.100:5000`

2. **Giriş Ekranı**: Varsayılan kullanıcı bilgileri:
   - **Kullanıcı Adı**: `grup11`
   - **Şifre**: `12345`

3. **Kontrol Paneli**: Başarılı girişin ardından terapi kontrolü ve izleme ekranına ulaşırsınız.

## Önemli Notlar

- Ana Windows uygulaması, EngineAPI ve mobil uygulama aynı yerel ağda olmalıdır
- MobileCommandServer Port 9000'de dinlemede olmalıdır
- EngineAPI Port 5000'de HTTP isteklerini kabul eder
- Fiziksel cihaz bağlantısı için ana Windows uygulaması gereklidir
- Simülasyon modu test için kullanılabilir

## Teknolojiler

- **Frontend**: React Native, Expo, TypeScript, TailwindCSS (NativeWind)
- **Backend**: ASP.NET Core 9.0, JWT Authentication
- **İletişim**: TCP Socket, REST API, Serial Communication
- **Platform**: iOS, Android (Expo Go ile test edilebilir)