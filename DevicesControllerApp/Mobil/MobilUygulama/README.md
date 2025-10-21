
## Projeyi Çalıştırma

Projeyi yerel makinenizde çalıştırmak için hem frontend hem de backend için ayrı adımları izlemeniz gerekmektedir.

### 1. Backend (.NET Web API)

Backend, `EngineAPI` klasöründe yer alan bir .NET projesidir.

**Gereksinimler:**

* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

**Kurulum ve Çalıştırma:**

1.  **Bağımlılıkları Yükleme:**
    Projenin ana dizinindeyken `EngineAPI` klasörüne gidin ve bağımlılıkları yükleyin.

    ```bash
    cd EngineAPI
    dotnet restore
    ```

2.  **API'yi Başlatma:**
    Aynı klasörde `dotnet run` komutunu çalıştırarak API'yi başlatın.

    ```bash
    dotnet run
    ```

    API varsayılan olarak `http://localhost:5000` veya `https://localhost:5001` portunda çalışmaya başlayacaktır.

### 2. Frontend (React Native - Expo)

Frontend, projenin ana dizininde bulunan bir React Native projesidir ve Expo kullanılarak geliştirilmiştir.

**Gereksinimler:**

* [Node.js](https://nodejs.org/) (LTS versiyonu önerilir)
* [Expo Go](https://expo.dev/client) uygulaması (mobil cihazınızda test etmek için)

**Kurulum ve Çalıştırma:**

1.  **Bağımlılıkları Yükleme:**
    Projenin ana dizinine gidin ve `npm` veya `yarn` kullanarak tüm bağımlılıkları yükleyin.

    ```bash
    npm install
    # veya
    yarn install
    ```

2.  **Uygulamayı Başlatma:**
    Kurulum tamamlandıktan sonra aşağıdaki komutlardan birini kullanarak uygulamayı başlatabilirsiniz:

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


    3. Uygulama Akışı
Mobil uygulamayı açtığınızda Bağlantı Ekranı sizi karşılar. Buraya, backend API'sinin çalıştığı bilgisayarın yerel ağdaki IP adresini ve portunu (5086) girin.

"İlerle" butonuna bastıktan sonra Giriş Ekranı'na yönlendirilirsiniz. Varsayılan kullanıcı bilgileri:

Kullanıcı Adı: grup11

Şifre: 12345

Başarılı girişin ardından ana kontrol paneline ulaşırsınız.