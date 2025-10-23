using System;
using System.Collections.Generic;
using System.IO.Ports; // Seri Port (RS232/RS485) iletişimi için
using System.Threading;

namespace DevicesControllerApp.Core
{
    // Cihazlarla tüm düşük seviyeli iletişimi yöneten sınıf
    public class DeviceManager : IDisposable
    {
        // Seri Port nesnesi
        private SerialPort _serialPort;
        // Bağlantı durumu
        public bool IsConnected { get; private set; } = false;

        // **1. Bağlantı Yönetimi Metodu**
        public bool Connect(string portName)
        {
            if (IsConnected) return true;

            try
            {
                // Seri Port ayarlarını donanımınıza göre kesinlikle güncelleyin!
                _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                _serialPort.Handshake = Handshake.None; 
                _serialPort.ReadTimeout = 500; // Okuma zaman aşımı (ms)
                _serialPort.WriteTimeout = 500; // Yazma zaman aşımı (ms)

                _serialPort.Open();
                IsConnected = _serialPort.IsOpen;
                
                if (IsConnected)
                {
                    // Opsiyonel: Bağlantı sonrası cihazdan durum sorgulama komutu gönderilebilir.
                    Console.WriteLine($"[INFO] Seri Port bağlantısı başarılı: {portName}");
                }

                return IsConnected;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                // Bağlantı hatasını çağırana (ServisController'a) fırlat
                throw new Exception($"Seri Port bağlantı hatası ({portName}): {ex.Message}");
            }
        }

        // **2. Motor Kontrolü (Manuel)**
        // [MOTOR_ID],[KOMUT],[HIZ],[MESAFE] formatında örnek komut gönderimi
        public void MoveMotorManual(string motorId, int speed, int distance)
        {
            if (!IsConnected) throw new InvalidOperationException("Cihaz bağlı değil. Motor komutu gönderilemez.");

            // Komut formatı: [Motor Adı],MOVE,[Hız],[Mesafe]\n (Örn: Servo_1,MOVE,1000,5000)
            string command = $"{motorId},MOVE,{speed},{distance}\r\n";
            
            SendCommand(command);
        }

        // **3. LoadCell Değerlerini Anlık Okuma**
        public double ReadLoadCellValue()
        {
            if (!IsConnected) return 0.0;
            
            try
            {
                string command = "READ_LOAD_CELL\r\n";
                // Gerçek projede: SendCommand ile komut gönderilir ve yanıtı okunur.
                // string response = SendCommandAndReadResponse(command);
                // return double.Parse(response); 

                // Simülasyon: Rastgele bir LoadCell değeri döndürüyoruz.
                Random rand = new Random();
                return 40.0 + (rand.NextDouble() * 10.0 - 5.0); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] LoadCell okuma hatası: {ex.Message}");
                return 0.0;
            }
        }

        // **4. Limit Switch Durumlarını Görüntüleme**
        public Dictionary<string, bool> GetLimitSwitchStatus()
        {
            if (!IsConnected) return new Dictionary<string, bool>();

            try
            {
                string command = "READ_ALL_LIMITS\r\n";
                // Gerçek projede: Cihazdan gelen, tüm switch durumlarını içeren yanıtı parse et.
                
                // Simülasyon: Rastgele bir veya iki switch'i aktif (true) yapma
                return new Dictionary<string, bool>
                {
                    // X Min %20 ihtimalle aktif (Limit Switch testi için)
                    {"X_MIN", new Random().Next(0, 5) == 0}, 
                    {"X_MAX", false},
                    {"Y_MIN", false},
                    {"Y_MAX", false},
                    // ... Diğer 13 motorun limit switch'leri buraya eklenmeli
                };
            }
            catch
            {
                return new Dictionary<string, bool>();
            }
        }

        // **5. Homing (Sıfırlama) İşlemi**
        public void StartHoming(string axis)
        {
            if (!IsConnected) throw new InvalidOperationException("Cihaz bağlı değil.");
            
            // Komut: HOMING,[EKSEN]\n (Örn: HOMING,ALL_AXES)
            string command = $"HOMING,{axis}\r\n";
            SendCommand(command);
            
            Console.WriteLine($"[TX] Homing komutu gönderildi: {axis}");
        }

        // **6. Kalibrasyon İşlemi**
        public void StartCalibration(string type)
        {
            if (!IsConnected) throw new InvalidOperationException("Cihaz bağlı değil.");
            
            // Komut: CALIBRATE,[TİP]\n (Örn: CALIBRATE,LOADCELL)
            string command = $"CALIBRATE,{type}\r\n";
            SendCommand(command);

            Console.WriteLine($"[TX] Kalibrasyon komutu gönderildi: {type}");
        }
        
        // **Seri Porta Komut Gönderme Yardımcı Metodu**
        private void SendCommand(string command)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                // Komutu Seri Porta yazar
                _serialPort.Write(command);
            }
            else
            {
                 throw new InvalidOperationException("Seri Port açık değil. Komut gönderilemedi.");
            }
        }
        
        // Not: Gerçek projelerde SendCommandAndReadResponse adında bir metotla yanıtı okumanız GEREKİR.

        // **7. Kaynakları Temizleme (IDisposable Uygulaması)**
        // ServisController kapanırken bu metot çağrılmalıdır.
        public void Dispose()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _serialPort.Dispose();
            }
            IsConnected = false;
        }
    }
}