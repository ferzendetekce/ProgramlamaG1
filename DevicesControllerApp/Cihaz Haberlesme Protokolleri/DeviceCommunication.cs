using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace RehabilitationSystem.Communication
{
    public class DeviceCommunication
    {
        private static DeviceCommunication _instance;
        private static readonly object _lock = new object();
        private Dictionary<byte, ManualResetEvent> _responseWaiters = new Dictionary<byte, ManualResetEvent>();
        private Dictionary<byte, byte[]> _responseData = new Dictionary<byte, byte[]>();
        private readonly object _responseLock = new object();
        private float _simulatedSpeed = 50.0f;
        private Dictionary<int, int> _simulatedMotorPositions = new Dictionary<int, int>();


        private SerialPort _serialPort;
        private Thread _readThread;
        private bool _isReading;
        private Queue<byte[]> _commandQueue;
        private readonly object _queueLock = new object();
        private List<byte> _rawRxBuffer = new List<byte>();
        private Queue<string> _errorHistory = new Queue<string>();
        private const int MAX_ERROR_HISTORY = 50;
        private readonly object _errorLock = new object();
        private string _lastError = string.Empty;

        // Buffer'lar
        private Queue<LoadCellDataPacket> _loadCellBuffer;
        private readonly object _bufferLock = new object();

        // Event'ler
        public event EventHandler<string> LogMessage;
        public event EventHandler<LoadCellDataEventArgs> LoadCellDataReceived;
        public event EventHandler<DeviceStatusEventArgs> DeviceStatusChanged;
        public event EventHandler<ErrorEventArgs> ErrorOccurred;
        public event EventHandler<ConnectionEventArgs> ConnectionStatusChanged;
        public event EventHandler<CommandResponseEventArgs> CommandResponseReceived;

        // Özellikler
        public bool IsConnected { get; private set; }
        public string CurrentPort { get; private set; }
        public int BaudRate { get; private set; }
        public int CommandTimeout { get; set; } = 3000; // ms

        public bool SimulationMode { get; set; } = false; // Test için

        // Singleton Instance
        public static DeviceCommunication Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DeviceCommunication();
                        }
                    }
                }
                return _instance;
            }
        }

        private DeviceCommunication()
        {
            // Buffer'ı oluşturuyoruz
            _loadCellBuffer = new Queue<LoadCellDataPacket>();
            _commandQueue = new Queue<byte[]>();
        }

        public bool OpenPort(string portName, int baudRate = 115200, Parity parity = Parity.None,
    int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            lock (_lock) // Thread safety için
            {
                try
                {
                    if (IsConnected) return true; // Zaten açıksa işlem yapma

                    InitializePort(); // Port nesnesini sıfırla/oluştur

                    _serialPort.PortName = portName;
                    _serialPort.BaudRate = baudRate;
                    _serialPort.Parity = parity;
                    _serialPort.DataBits = dataBits;
                    _serialPort.StopBits = stopBits;

                    _serialPort.Open();

                    CurrentPort = portName;
                    BaudRate = baudRate;
                    IsConnected = true;

                    // Okuma Thread'ini başlat (Henüz içini doldurmadık ama start veriyoruz)
                    StartReadingThread();

                    // Event tetikle: Bağlantı başarılı
                    OnConnectionStatusChanged(true, portName);
                    LogCommunication($"Port açıldı: {portName} @ {baudRate}", false);

                    return true;
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    HandleCommunicationError(ex, "OpenPort");
                    return false;
                }
            }
        }

        public bool ClosePort()
        {
            lock (_lock)
            {
                try
                {
                    if (!IsConnected) return true;

                    // Önce okuma thread'ini durdur
                    StopReadingThread();

                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        // Dtr ve Rts pinlerini kapatmak bazen cihazı resetlemek için gerekebilir
                        _serialPort.DtrEnable = false;
                        _serialPort.RtsEnable = false;

                        // Bufferları temizle
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();

                        _serialPort.Close();
                    }

                    IsConnected = false;

                    // Event tetikle: Bağlantı kesildi
                    OnConnectionStatusChanged(false, CurrentPort);
                    LogCommunication("Port kapatıldı.", false);

                    return true;
                }
                catch (Exception ex)
                {
                    HandleCommunicationError(ex, "ClosePort");
                    return false;
                }
            }
        }

        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public bool IsPortOpen()
        {
            return _serialPort != null && _serialPort.IsOpen;
        }

        private void InitializePort()
        {
            // Eğer port daha önce oluşturulmuşsa temizleyelim
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen) _serialPort.Close();
                _serialPort.Dispose();
            }

            _serialPort = new SerialPort();

            // Temel timeout ayarları (okuma/yazma kilitlenmesin diye)
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
        }

        public ushort CalculateCRC16(byte[] data)
        {
            ushort crc = 0xFFFF;

            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i]); // Byte'ı XOR'la

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        public byte CalculateChecksum(byte[] data)
        {
            byte sum = 0;
            foreach (byte b in data)
            {
                unchecked { sum += b; }
            }
            return sum;
        }

        // CRC Doğrulama Yardımcısı
        public bool VerifyCRC16(byte[] data, ushort receivedCrc)
        {
            ushort calculated = CalculateCRC16(data);
            return calculated == receivedCrc;
        }

        private void DispatchReceivedPacket(byte commandCode, byte[] payload)
        {
            // ✅ DEBUG: Gelen komut kodunu logla
            LogCommunication($"[DEBUG] DispatchReceivedPacket: commandCode=0x{commandCode:X2}, payloadLen={payload?.Length ?? 0}");

            // Önce yanıt bekleyen var mı kontrol et
            lock (_responseLock)
            {
                // ✅ DEBUG: Bekleyen waiter'ları logla
                LogCommunication($"[DEBUG] Bekleyen waiter sayısı: {_responseWaiters.Count}");

                if (_responseWaiters.ContainsKey(commandCode))
                {
                    LogCommunication($"[DEBUG] ✓ Waiter bulundu! Komut 0x{commandCode:X2}");
                    _responseData[commandCode] = payload;
                    _responseWaiters[commandCode].Set();
                    return;
                }
                else
                {
                    LogCommunication($"[DEBUG] ✗ Waiter BULUNAMADI! Komut 0x{commandCode:X2}", true);
                }
            }

            // Yanıt bekleyen yoksa normal işlem
            CommandCode code = (CommandCode)commandCode;

            switch (code)
            {
                case CommandCode.ReadLoadCell:
                    ParseLoadCellData(payload);
                    break;

                case CommandCode.ReadStatus:
                    // ParseDeviceStatus(payload);
                    break;

                default:
                    OnCommandResponseReceived(commandCode, payload);
                    break;
            }
        }

        public bool VerifyChecksum(byte[] data, byte receivedChecksum)
        {
            return false;
        }

        public bool SendCommand(byte commandCode, byte[] data = null)
        {
            try
            {
                // 1. Paketi oluştur
                byte[] packet = BuildCommandPacket(commandCode, data);

                // 2. Porta yaz
                return WriteToPort(packet);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Komut gönderme hatası: {ex.Message}", ErrorLevel.Error);
                return false;
            }
        }

        public byte[] SendCommandAndWaitResponse(byte commandCode, byte[] data = null, int timeoutMs = 0)
        {
            if (timeoutMs == 0)
                timeoutMs = CommandTimeout;

            ManualResetEvent waitHandle = null;

            try
            {
                if (SimulationMode)
                {
                    LogCommunication($"[SİMÜLASYON] Komut 0x{commandCode:X2} gönderildi...");
                    Thread.Sleep(100);
                    return GenerateSimulatedResponse(commandCode, data);
                }

                waitHandle = new ManualResetEvent(false);

                // 1. Waiter'ı ekle
                lock (_responseLock)
                {
                    LogCommunication($"[DEBUG] Waiter ekleniyor: 0x{commandCode:X2}");
                    _responseWaiters[commandCode] = waitHandle;

                    if (_responseData.ContainsKey(commandCode))
                        _responseData.Remove(commandCode);
                }

                // 2. Paketi gönder
                LogCommunication($"[DEBUG] Paket gönderiliyor...");
                if (!SendCommand(commandCode, data))
                {
                    LogCommunication($"[DEBUG] ✗ SendCommand başarısız!");
                    lock (_responseLock)
                    {
                        _responseWaiters.Remove(commandCode);
                    }
                    return null;
                }

                LogCommunication($"[DEBUG] Paket gönderildi, yanıt bekleniyor (timeout: {timeoutMs}ms)...");

                // 3. Yanıtı bekle
                bool received = waitHandle.WaitOne(timeoutMs);

                LogCommunication($"[DEBUG] Bekleme bitti: received={received}");

                // 4. Yanıtı al ve waiter'ı temizle
                lock (_responseLock)
                {
                    _responseWaiters.Remove(commandCode);  // ← ÖNCE WAİTER'I SİL

                    if (received && _responseData.ContainsKey(commandCode))
                    {
                        byte[] response = _responseData[commandCode];
                        _responseData.Remove(commandCode);

                        LogCommunication($"[DEBUG] ✓ Yanıt alındı! Uzunluk: {response.Length}");
                        return response;
                    }
                }

                LogCommunication($"[ERROR] TIMEOUT! Komut 0x{commandCode:X2} için {timeoutMs}ms içinde yanıt gelmedi.", true);
                return null;
            }
            catch (Exception ex)
            {
                LogCommunication($"[ERROR] SendCommandAndWaitResponse hatası: {ex.Message}", true);

                // Hata durumunda waiter'ı temizle
                if (waitHandle != null)
                {
                    lock (_responseLock)
                    {
                        _responseWaiters.Remove(commandCode);
                    }
                }

                return null;
            }
        }

        private byte[] GenerateSimulatedResponse(byte commandCode, byte[] sentData)
        {
            CommandCode cmd = (CommandCode)commandCode;

            switch (cmd)
            {
                case CommandCode.Connect:
                    return new byte[] { 0x01 };

                case CommandCode.Disconnect:
                    return new byte[] { 0x01 };

                case CommandCode.SetSpeed:
                    // Gönderilen hızı kaydet
                    if (sentData != null && sentData.Length >= 4)
                    {
                        _simulatedSpeed = BitConverter.ToSingle(sentData, 0);
                    }
                    return new byte[] { 0x01 };

                case CommandCode.GetSpeed:
                    // Kaydedilmiş hızı döndür
                    return BitConverter.GetBytes(_simulatedSpeed);

                case CommandCode.SetStepMotor:
                    // Motor hareketini simüle et
                    if (sentData != null && sentData.Length >= 5)
                    {
                        int motorIndex = sentData[0];
                        int steps = BitConverter.ToInt32(sentData, 1);

                        if (!_simulatedMotorPositions.ContainsKey(motorIndex))
                            _simulatedMotorPositions[motorIndex] = 0;

                        _simulatedMotorPositions[motorIndex] += steps;

                        LogCommunication($"[SİMÜLASYON] Motor {motorIndex}: {_simulatedMotorPositions[motorIndex] - steps} → {_simulatedMotorPositions[motorIndex]}");
                    }
                    return new byte[] { 0x01 };

                case CommandCode.GetMotorPosition:
                    // Motor pozisyonunu döndür
                    int requestedMotor = 0;

                    if (sentData != null && sentData.Length >= 1)
                    {
                        requestedMotor = sentData[0];
                    }

                    if (!_simulatedMotorPositions.ContainsKey(requestedMotor))
                        _simulatedMotorPositions[requestedMotor] = 0;

                    return BitConverter.GetBytes(_simulatedMotorPositions[requestedMotor]);

                case CommandCode.StartTherapy:
                    return new byte[] { 0x01 };

                case CommandCode.StopTherapy:
                    return new byte[] { 0x01 };

                case CommandCode.PauseTherapy:
                    return new byte[] { 0x01 };

                case CommandCode.ResumeTherapy:
                    return new byte[] { 0x01 };

                case CommandCode.EmergencyStop:
                    LogCommunication("[SİMÜLASYON] ACİL DURDURMA aktif!");
                    return new byte[] { 0x01 };

                default:
                    LogCommunication($"[SİMÜLASYON] Bilinmeyen komut: 0x{commandCode:X2}");
                    return new byte[] { 0x00 };
            }
        }

        private byte[] BuildCommandPacket(byte commandCode, byte[] data)
        {
            // Örnek Protokol Yapısı:
            // [0] Header 1 (0x55)
            // [1] Header 2 (0xAA)
            // [2] Data Length (Komut + Data uzunluğu)
            // [3] Command Code
            // [4...] Data (Varsa)
            // [Son-1] CRC Low
            // [Son] CRC High

            List<byte> packet = new List<byte>();

            // 1. Başlıklar (Preamble)
            packet.Add(0x55);
            packet.Add(0xAA);

            // 2. Veri Hazırlığı
            int dataLength = (data != null) ? data.Length : 0;

            // Uzunluk: Komut (1 byte) + Data Uzunluğu
            packet.Add((byte)(1 + dataLength));

            // 3. Komut
            packet.Add(commandCode);

            // 4. Veri (Payload)
            if (data != null && data.Length > 0)
            {
                packet.AddRange(data);
            }

            // 5. CRC Hesaplama (Başlıklar hariç, Length'den itibaren hesaplanır - Cihaz protokolüne göre değişebilir)
            // Burada tüm paket içeriği üzerinden hesaplıyoruz (Headerlar hariç pratik bir yaklaşım)
            byte[] payloadForCrc = packet.GetRange(2, packet.Count - 2).ToArray();
            ushort crc = CalculateCRC16(payloadForCrc);

            packet.Add((byte)(crc & 0xFF));        // Low Byte
            packet.Add((byte)((crc >> 8) & 0xFF)); // High Byte

            return packet.ToArray();
        }

        private bool WriteToPort(byte[] data)
        {
            if (!IsConnected || _serialPort == null || !_serialPort.IsOpen)
            {
                LogCommunication("Port açık değil, veri gönderilemedi.", true);
                return false;
            }

            lock (_lock) // Thread safety: Aynı anda tek bir yazma işlemi
            {
                try
                {
                    // Yazmadan önce buffer'ı temizlemek opsiyoneldir, 
                    // ama yanıt bekleyen sistemlerde eski veriyi temizlemek iyidir.
                    // _serialPort.DiscardOutBuffer(); 

                    _serialPort.Write(data, 0, data.Length);

                    // Debug için log (Canlı sistemde performans için kapatılabilir)
                    string hexData = BitConverter.ToString(data);
                    LogCommunication($"GÖNDERİLDİ: {hexData}");

                    return true;
                }
                catch (Exception ex)
                {
                    HandleCommunicationError(ex, "WriteToPort");
                    return false;
                }
            }
        }

        private void AddToCommandQueue(byte[] command)
        {
            lock (_queueLock)
            {
                _commandQueue.Enqueue(command);
                LogCommunication($"Komut kuyruğa eklendi. Kuyruk boyutu: {_commandQueue.Count}");
            }
        }

        private void ProcessCommandQueue()
        {
            lock (_queueLock)
            {
                if (_commandQueue.Count == 0)
                    return;

                LogCommunication($"Komut kuyruğu işleniyor... Bekleyen komut sayısı: {_commandQueue.Count}");

                while (_commandQueue.Count > 0)
                {
                    byte[] command = _commandQueue.Dequeue();

                    if (command != null && command.Length > 0)
                    {
                        // Komutu gönder
                        bool success = WriteToPort(command);

                        if (!success)
                        {
                            LogCommunication("✗ Kuyruktan komut gönderimi başarısız!", true);
                            break; // Hata durumunda kuyruğu durur
                        }

                        // Komutlar arası kısa bekleme
                        Thread.Sleep(50);
                    }
                }

                LogCommunication("Komut kuyruğu işleme tamamlandı.");
            }
        }

        public void ClearCommandQueue()
        {
            lock (_queueLock)
            {
                _commandQueue.Clear();
                LogCommunication("Komut kuyruğu temizlendi.");
            }
        }

        public int GetCommandQueueSize()
        {
            lock (_queueLock)
            {
                return _commandQueue.Count;
            }
        }
        private void StartReadingThread()
        {
            if (_readThread != null && _readThread.IsAlive)
                return;

            _isReading = true;
            _readThread = new Thread(ReadDataContinuously);
            _readThread.IsBackground = true; // Uygulama kapanırsa thread de kapansın
            _readThread.Name = "SerialReadThread";
            _readThread.Start();
        }

        private void StopReadingThread()
        {
            _isReading = false;

            // Thread'in durmasını bekle (maksimum 500ms)
            if (_readThread != null && _readThread.IsAlive)
            {
                _readThread.Join(500);
            }
        }

        private void ReadDataContinuously()
        {
            LogCommunication(">>> OKUMA DÖNGÜSÜ BAŞLADI <<<"); // EKLE

            while (_isReading)
            {
                try
                {
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        int bytesToRead = _serialPort.BytesToRead;

                        if (bytesToRead > 0)
                        {
                            LogCommunication($">>> {bytesToRead} BYTE GELDİ <<<"); // EKLE

                            byte[] chunk = ReadFromPort(bytesToRead, 100);

                            if (chunk != null && chunk.Length > 0)
                            {
                                string hexData = BitConverter.ToString(chunk);
                                LogCommunication($"ALINDI: {hexData}"); // EKLE

                                _rawRxBuffer.AddRange(chunk);
                                ProcessReceivedData(null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogCommunication("Okuma hatası: " + ex.Message, true);
                }

                Thread.Sleep(10);
            }

            LogCommunication(">>> OKUMA DÖNGÜSÜ BİTTİ <<<"); // EKLE
        }

        private byte[] ReadFromPort(int bytesToRead, int timeoutMs)
        {
            byte[] buffer = new byte[bytesToRead];
            try
            {
                // Okuma işlemi
                int readCount = _serialPort.Read(buffer, 0, bytesToRead);

                // Eğer istenenden az okunduysa array'i küçült (nadiren gerekir)
                if (readCount < bytesToRead)
                {
                    byte[] actualData = new byte[readCount];
                    Array.Copy(buffer, actualData, readCount);
                    return actualData;
                }
                return buffer;
            }
            catch (TimeoutException)
            {
                return null; // Timeout normaldir, veri yok demektir.
            }
            catch (Exception ex)
            {
                HandleCommunicationError(ex, "ReadFromPort");
                return null;
            }
        }

        private void ProcessReceivedData(byte[] unusedData)
        {
            // Minimum paket boyutu: Header(2) + Len(1) + Cmd(1) + CRC(2) = 6 byte
            while (_rawRxBuffer.Count >= 6)
            {
                // 1. Başlık Kontrolü (0x55, 0xAA)
                if (_rawRxBuffer[0] == 0x55 && _rawRxBuffer[1] == 0xAA)
                {
                    // 2. Uzunluk Bilgisini Al (3. byte uzunluk bilgisidir)
                    // Protokolümüze göre: Len = Komut(1) + Data(N)
                    byte packetLength = _rawRxBuffer[2];

                    // Toplam paket boyutu = Header(2) + Len(1) + Payload(Len) + CRC(2)
                    int totalExpectedLength = 2 + 1 + packetLength + 2;

                    // 3. Yeterli veri geldi mi?
                    if (_rawRxBuffer.Count >= totalExpectedLength)
                    {
                        // Paketi geçici diziye al
                        byte[] packet = _rawRxBuffer.GetRange(0, totalExpectedLength).ToArray();

                        // 4. CRC Kontrolü
                        // Son 2 byte CRC'dir
                        ushort receivedCrc = (ushort)(packet[packet.Length - 2] | (packet[packet.Length - 1] << 8));

                        // CRC hesaplanacak kısım: Headerlar hariç, Length byte'ından itibaren CRC öncesine kadar
                        byte[] dataToVerify = new byte[totalExpectedLength - 4];
                        Array.Copy(packet, 2, dataToVerify, 0, totalExpectedLength - 4);

                        ushort calculatedCrc = CalculateCRC16(dataToVerify);

                        if (calculatedCrc == receivedCrc)
                        {
                            // --- GEÇERLİ PAKET BULUNDU ---
                            LogCommunication("✓ CRC DOĞRU - Paket işleniyor", false);

                            byte commandCode = packet[3]; // Komut kodu

                            // Payload verisini ayıkla (Komut'tan sonra, CRC'den önce)
                            int payloadSize = packetLength - 1;
                            byte[] payload = null;

                            if (payloadSize > 0)
                            {
                                payload = new byte[payloadSize];
                                Array.Copy(packet, 4, payload, 0, payloadSize);
                            }

                            // Paketi ilgili yere yönlendir
                            DispatchReceivedPacket(commandCode, payload);

                            // İşlenen paketi buffer'dan sil
                            _rawRxBuffer.RemoveRange(0, totalExpectedLength);
                        }
                        else
                        {
                            // ⚠️ CRC HATASI - TEST MODU: Yine de işle
                            LogCommunication($"✗ CRC HATASI! Hesaplanan: 0x{calculatedCrc:X4}, Gelen: 0x{receivedCrc:X4}", true);
                            LogCommunication(">>> TEST MODU: CRC yanlış ama paketi yine de işliyorum <<<", true);

                            // Paketi yine de işle (TEST İÇİN)
                            byte commandCode = packet[3];
                            int payloadSize = packetLength - 1;
                            byte[] payload = null;

                            if (payloadSize > 0)
                            {
                                payload = new byte[payloadSize];
                                Array.Copy(packet, 4, payload, 0, payloadSize);
                            }

                            // Paketi dispatch et
                            DispatchReceivedPacket(commandCode, payload);

                            // Buffer'dan sil
                            _rawRxBuffer.RemoveRange(0, totalExpectedLength);
                        }
                    }
                    else
                    {
                        // Başlık var ama paketin devamı henüz gelmedi
                        // Döngüden çık, sonraki okumayı bekle
                        break;
                    }
                }
                else
                {
                    // Başlık eşleşmedi, buffer'ın başındaki çöp veriyi sil
                    _rawRxBuffer.RemoveAt(0);
                }
            }
        }


        public bool RequestLoadCellData()
        {
            LogCommunication("LoadCell veri istemi gönderiliyor...");
            return SendCommand((byte)CommandCode.ReadLoadCell);
        }

        public bool Connect()
        {
            LogCommunication("Cihaza bağlanılıyor...");
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.Connect, null, 2000);

            if (response != null)
            {
                LogCommunication("✓ Cihaz bağlantısı onaylandı!");
                return true;
            }
            else
            {
                LogCommunication("✗ Cihaz bağlantısı başarısız (timeout)!", true);
                return false;
            }
        }

        public bool Disconnect()
        {
            LogCommunication("Cihaz bağlantısı kesiliyor...");
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.Disconnect, null, 2000);

            if (response != null)
            {
                LogCommunication("✓ Cihaz bağlantısı kesildi!");
                return true;
            }
            else
            {
                LogCommunication("✗ Disconnect timeout!", true);
                return false;
            }
        }

        public bool SetSpeedWithConfirmation(double speed)
        {
            LogCommunication($"Hız ayarlanıyor: {speed}");

            byte[] speedData = BitConverter.GetBytes((float)speed);
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.SetSpeed, speedData, 1500);

            if (response != null)
            {
                LogCommunication($"✓ Hız başarıyla {speed} olarak ayarlandı!");
                return true;
            }
            else
            {
                LogCommunication("✗ Hız ayarlama timeout!", true);
                return false;
            }
        }

        public double GetCurrentSpeed()
        {
            LogCommunication("Mevcut hız sorgulanıyor...");

            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.GetSpeed, null, 3000);

            if (response != null && response.Length >= 4)
            {
                float speed = BitConverter.ToSingle(response, 0);
                LogCommunication($"✓ Mevcut hız: {speed}");
                return speed;
            }
            else
            {
                LogCommunication("✗ Hız okuma başarısız (timeout veya geçersiz veri)!", true);
                return -1;
            }
        }



        private void ParseLoadCellData(byte[] payload)
        {
            // Beklenen veri boyutu kontrolü (4 float + 1 int = 20 byte)
            if (payload == null || payload.Length < 20)
            {
                LogCommunication("Eksik LoadCell verisi alındı.", true);
                return;
            }

            try
            {
                LoadCellDataPacket packet = new LoadCellDataPacket();

                // Byte dizisinden sayısal değerlere dönüşüm (Little Endian varsayımı)
                // Not: Cihaz 'double' gönderiyorsa ToDouble, 'float' gönderiyorsa ToSingle kullanılır.
                // Genelde mikrodenetleyicilerde float (4 byte) tercih edilir.
                packet.RightHeel = BitConverter.ToSingle(payload, 0);
                packet.RightToe = BitConverter.ToSingle(payload, 4);
                packet.LeftHeel = BitConverter.ToSingle(payload, 8);
                packet.LeftToe = BitConverter.ToSingle(payload, 12);
                packet.Index = BitConverter.ToInt32(payload, 16);

                packet.Timestamp = DateTime.Now;

                // Hesaplanan Değerler (PC tarafında hesaplamak daha iyidir)
                double totalRight = packet.RightHeel + packet.RightToe;
                double totalLeft = packet.LeftHeel + packet.LeftToe;
                double totalWeight = totalRight + totalLeft;

                // Denge Oranı (%50 - %50 ideal)
                if (totalWeight > 0)
                    packet.WeightBalance = (totalRight / totalWeight) * 100;
                else
                    packet.WeightBalance = 50.0;

                // 1. Buffer'a ekle
                AddToLoadCellBuffer(packet);

                // 2. Event fırlat (Canlı grafik çizimi için anlık veri)
                OnLoadCellDataReceived(packet);
            }
            catch (Exception ex)
            {
                LogCommunication("Parse hatası: " + ex.Message, true);
            }
        }

        private const int MAX_BUFFER_SIZE = 5000; // Örn: 50 saniyelik veri (100Hz ise)

        private void AddToLoadCellBuffer(LoadCellDataPacket packet)
        {
            lock (_bufferLock)
            {
                // Kapasite dolduysa en eski veriyi at
                if (_loadCellBuffer.Count >= MAX_BUFFER_SIZE)
                {
                    _loadCellBuffer.Dequeue();
                }

                _loadCellBuffer.Enqueue(packet);
            }
        }

        // Dış dünyanın veriyi çekmesi için metotlar:

        public LoadCellDataPacket GetLatestLoadCellData()
        {
            lock (_bufferLock)
            {
                if (_loadCellBuffer.Count > 0)
                {
                    // Son elemanı döndür ama kuyruktan silme (Peek)
                    // Eğer son elemanı almak için kuyruğu "Last" ile sorgularsak O(n) olabilir,
                    // Queue yapısında genelde son ekleneni almak için ToArray maliyetlidir.
                    // Performans için "son eklenen" değişkeni tutmak daha iyidir ama
                    // basitlik adına ToArray().Last() yerine şunu yapabiliriz:

                    // Not: Queue FIFO (İlk giren ilk çıkar) yapısındadır.
                    // En son eklenen veriye Queue üzerinden doğrudan erişim yoktur.
                    // Bu yüzden _lastPacket diye bir değişken tutup onu dönmek en hızlısıdır.
                    // Ancak şimdilik güvenli yol (kopya alıp sonuncuya bakmak):
                    return _loadCellBuffer.ToArray()[_loadCellBuffer.Count - 1];
                }
                return null;
            }
        }

        // Uygulamanın belirli bir miktarda veriyi (örn: son 100 veri) çekmesi için
        public List<LoadCellDataPacket> GetLoadCellBuffer(int count)
        {
            lock (_bufferLock)
            {
                var allData = _loadCellBuffer.ToArray();

                if (allData.Length <= count)
                {
                    return new List<LoadCellDataPacket>(allData);
                }
                else
                {
                    // Son 'count' kadar veriyi al
                    int startIndex = allData.Length - count;
                    List<LoadCellDataPacket> result = new List<LoadCellDataPacket>();
                    for (int i = startIndex; i < allData.Length; i++)
                    {
                        result.Add(allData[i]);
                    }
                    return result;
                }
            }
        }

        public void ClearLoadCellBuffer()
        {
            lock (_bufferLock)
            {
                _loadCellBuffer.Clear();
            }
        }

        private void OnLoadCellDataReceived(LoadCellDataPacket data)
        {
            // Event null kontrolü (? operatörü ile)
            LoadCellDataReceived?.Invoke(this, new LoadCellDataEventArgs
            {
                Data = data,
                Timestamp = DateTime.Now
            });
        }

        public bool MoveMotor(int motorIndex, int steps)
        {
            LogCommunication($"Motor {motorIndex} -> {steps} adım ilerletiliyor...");

            List<byte> payload = new List<byte>();
            payload.Add((byte)motorIndex);
            payload.AddRange(BitConverter.GetBytes(steps));

            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.SetStepMotor, payload.ToArray(), 3000);

            if (response != null)
            {
                LogCommunication($"✓ Motor {motorIndex} başarıyla {steps} adım ilerledi!");
                return true;
            }
            else
            {
                LogCommunication($"✗ Motor hareket timeout!", true);
                return false;
            }
        }

        public int GetMotorPosition(int motorIndex)
        {
            LogCommunication($"Motor {motorIndex} pozisyonu sorgulanıyor...");

            byte[] motorIndexData = new byte[] { (byte)motorIndex };
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.GetMotorPosition, motorIndexData, 3000);

            if (response != null && response.Length >= 4)
            {
                int position = BitConverter.ToInt32(response, 0);
                LogCommunication($"✓ Motor {motorIndex} pozisyonu: {position}");
                return position;
            }
            else
            {
                LogCommunication($"✗ Motor pozisyon okuma başarısız!", true);
                return -1;
            }
        }


        // ✅ DOĞRU VERSIYONDEVICE

<<<<<<< HEAD


        public bool StartTherapy()
=======
        

            public bool StartTherapy()
>>>>>>> 97da60aa201966cefec4af7dd41e7e826ba96776
        {
            LogCommunication("Terapi başlatılıyor...");

            // ✅ YANIT BEKLE!
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.StartTherapy, null, 2000);

            if (response != null)
            {
                LogCommunication("✓ Terapi başlatıldı!");
                return true;
            }
            else
            {
                LogCommunication("✗ Terapi başlatma timeout!", true);
                return false;
            }
        }

        public bool StopTherapy()
        {
            LogCommunication("Terapi durduruluyor...");

            // ✅ YANIT BEKLE!
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.StopTherapy, null, 2000);

            if (response != null)
            {
                LogCommunication("✓ Terapi durduruldu!");
                return true;
            }
            else
            {
                LogCommunication("✗ Terapi durdurma timeout!", true);
                return false;
            }
        }

        public bool PauseTherapy()
        {
            LogCommunication("Terapi duraklatılıyor...");

            // ✅ YANIT BEKLE!
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.PauseTherapy, null, 2000);

            if (response != null)
            {
                LogCommunication("✓ Terapi duraklatıldı!");
                return true;
            }
            else
            {
                LogCommunication("✗ Terapi duraklatma timeout!", true);
                return false;
            }
        }

        public bool ResumeTherapy()
        {
            LogCommunication("Terapi devam ettiriliyor...");

            // ✅ YANIT BEKLE!
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.ResumeTherapy, null, 2000);

            if (response != null)
            {
                LogCommunication("✓ Terapi devam ediyor!");
                return true;
            }
            else
            {
                LogCommunication("✗ Terapi devam ettirme timeout!", true);
                return false;
            }
        }

        public bool EmergencyStop()
        {
            LogCommunication("!!! ACİL DURDURMA !!!", true);

            // ✅ YANIT BEKLE (Acil stop için de onay almak önemli!)
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.EmergencyStop, null, 2000);

            if (response != null)
            {
                LogCommunication("✓ Acil durdurma gerçekleşti!");
                return true;
            }
            else
            {
                LogCommunication("✗ Acil durdurma timeout!", true);
                return false;
            }
        }

        public bool SetSpeed(double speed)
        {
            // Hızı float (4 byte) olarak gönderiyoruz
            return SendCommand((byte)CommandCode.SetSpeed, BitConverter.GetBytes((float)speed));
        }

        public bool SetShoeSize(int size)
        {
            return SendCommand((byte)CommandCode.SetShoeSize, BitConverter.GetBytes(size));
        }

        public bool SetSupportBarHeight(double height)
        {
            return SendCommand((byte)CommandCode.SetSupportBar, BitConverter.GetBytes((float)height));
        }

        public bool SetWeightReduction(double weight)
        {
            return SendCommand((byte)CommandCode.SetWeightReduction, BitConverter.GetBytes((float)weight));
        }

        public bool SetWinchPosition(bool up)
        {
            byte[] data = new byte[] { (byte)(up ? 0x01 : 0x00) };
            return SendCommand((byte)CommandCode.SetWinch, data);
        }

        public bool LoadPattern(byte[] patternData)
        {
            if (patternData == null || patternData.Length == 0)
                return false;

            return SendCommand((byte)CommandCode.LoadPattern, patternData);
        }

        public bool HomeDevice()
        {
            return SendCommand((byte)CommandCode.HomeDevice);
        }

        public bool SetServoMotorPosition(int motorIndex, int position)
        {
            List<byte> payload = new List<byte>();
            payload.Add((byte)motorIndex);
            payload.AddRange(BitConverter.GetBytes(position));
            return SendCommand((byte)CommandCode.SetServoMotor, payload.ToArray());
        }

        public bool SetStepMotorPosition(int motorIndex, int steps)
        {
            List<byte> payload = new List<byte>();
            payload.Add((byte)motorIndex);
            payload.AddRange(BitConverter.GetBytes(steps));
            return SendCommand((byte)CommandCode.SetStepMotor, payload.ToArray());
        }

        public int GetServoMotorPosition(int motorIndex)
        {
            LogCommunication($"Servo motor {motorIndex} pozisyonu sorgulanıyor...");

            byte[] motorIndexData = new byte[] { (byte)motorIndex };
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.GetServoPosition, motorIndexData, 2000);

            if (response != null && response.Length >= 4)
            {
                int position = BitConverter.ToInt32(response, 0);
                LogCommunication($"✓ Servo motor {motorIndex} pozisyonu: {position}");
                return position;
            }
            else
            {
                LogCommunication($"✗ Servo motor pozisyon okuma başarısız!", true);
                return -1;
            }
        }

        public int GetStepMotorPosition(int motorIndex)
<<<<<<< HEAD
        {
=======
        {        
>>>>>>> 97da60aa201966cefec4af7dd41e7e826ba96776
            return GetMotorPosition(motorIndex);
        }

        public bool[] GetAllServoMotorPositions()
        {
            LogCommunication("Tüm servo motor pozisyonları okunuyor...");

            const int SERVO_MOTOR_COUNT = 7; // 7 adet servo motor
            bool[] positions = new bool[SERVO_MOTOR_COUNT];

            for (int i = 0; i < SERVO_MOTOR_COUNT; i++)
            {
                int pos = GetServoMotorPosition(i);
                positions[i] = (pos != -1); // Başarılı okuma = true
            }

            return positions;
        }

        public int[] GetAllStepMotorPositions()
        {
            LogCommunication("Tüm step motor pozisyonları okunuyor...");

            const int STEP_MOTOR_COUNT = 10; // 10 adet step motor
            int[] positions = new int[STEP_MOTOR_COUNT];

            for (int i = 0; i < STEP_MOTOR_COUNT; i++)
            {
                positions[i] = GetMotorPosition(i);
            }

            return positions;
        }

        public bool[] ReadLimitSwitches()
        {
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.ReadLimitSwitch, null, 2000);

            if (response != null && response.Length > 0)
            {
                bool[] switches = new bool[response.Length];
                for (int i = 0; i < response.Length; i++)
                {
                    switches[i] = (response[i] == 0x01);
                }
                return switches;
            }
            return null;
        }

        public double[] ReadAllLoadCells()
        {
            LogCommunication("Tüm LoadCell verileri okunuyor...");

            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.ReadLoadCell, null, 2000);

            if (response != null && response.Length >= 20)
            {
                // 5 kanal: RightHeel, RightToe, LeftHeel, LeftToe, Center (varsayılan)
                double[] loadCells = new double[5];

                loadCells[0] = BitConverter.ToSingle(response, 0);  // RightHeel
                loadCells[1] = BitConverter.ToSingle(response, 4);  // RightToe
                loadCells[2] = BitConverter.ToSingle(response, 8);  // LeftHeel
                loadCells[3] = BitConverter.ToSingle(response, 12); // LeftToe
                loadCells[4] = BitConverter.ToSingle(response, 16); // Center (opsiyonel)

                LogCommunication($"✓ LoadCell verileri alındı: RH={loadCells[0]:F2}, RT={loadCells[1]:F2}, LH={loadCells[2]:F2}, LT={loadCells[3]:F2}");

                return loadCells;
            }
            else
            {
                LogCommunication("✗ LoadCell veri okuma başarısız!", true);
                return null;
            }
        }

        public double ReadLoadCell(int channel)
        {
            if (channel < 0 || channel > 4)
            {
                LogCommunication($"✗ Geçersiz LoadCell kanalı: {channel}", true);
                return -1;
            }

            LogCommunication($"LoadCell kanal {channel} okunuyor...");

            byte[] channelData = new byte[] { (byte)channel };
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.ReadLoadCell, channelData, 2000);

            if (response != null && response.Length >= 4)
            {
                float value = BitConverter.ToSingle(response, 0);
                LogCommunication($"✓ LoadCell kanal {channel}: {value:F2}");
                return value;
            }
            else
            {
                LogCommunication($"✗ LoadCell kanal {channel} okuma başarısız!", true);
                return -1;
            }
        }

        public Dictionary<string, int> GetAllPositionSensors()
        {
            LogCommunication("Tüm pozisyon sensörleri okunuyor...");

            Dictionary<string, int> sensors = new Dictionary<string, int>();

            // Servo motorları ekle
            for (int i = 0; i < 7; i++)
            {
                int pos = GetServoMotorPosition(i);
                sensors[$"Servo_{i}"] = pos;
            }

            // Step motorları ekle
            for (int i = 0; i < 10; i++)
            {
                int pos = GetMotorPosition(i);
                sensors[$"Step_{i}"] = pos;
            }

            LogCommunication($"✓ Toplam {sensors.Count} pozisyon sensörü okundu.");

            return sensors;
        }

        public DeviceStatus QueryDeviceStatus()
        {
            byte[] response = SendCommandAndWaitResponse((byte)CommandCode.ReadStatus, null, 2000);

            if (response != null && response.Length >= 10)
            {
                DeviceStatus status = new DeviceStatus();
                status.IsReady = (response[0] == 0x01);
                status.IsRunning = (response[1] == 0x01);
                status.IsEmergencyStopped = (response[2] == 0x01);
                // ... daha fazla veri parse et

                return status;
            }

            return null;
        }

        public bool IsDeviceReady()
        {
            DeviceStatus status = QueryDeviceStatus();

            if (status != null)
            {
                bool ready = status.IsReady && !status.IsEmergencyStopped;
                LogCommunication($"Cihaz durumu: {(ready ? "Hazır ✓" : "Hazır değil ✗")}");
                return ready;
            }

            LogCommunication("✗ Cihaz durum sorgulaması başarısız!", true);
            return false;
        }

        public string GetDeviceFirmwareVersion()
        {
            LogCommunication("Firmware versiyonu sorgulanıyor...");

            // Firmware version komutu (CommandCode'a eklenebilir: GetFirmwareVersion = 0x60)
            byte[] response = SendCommandAndWaitResponse(0x60, null, 2000);

            if (response != null && response.Length > 0)
            {
                // Versiyon formatı: "v1.2.3" (string olarak)
                string version = System.Text.Encoding.ASCII.GetString(response).Trim('\0');
                LogCommunication($"✓ Firmware versiyonu: {version}");
                return version;
            }
            else
            {
                LogCommunication("✗ Firmware versiyon okuma başarısız!", true);
                return "Unknown";
            }
        }

        public Dictionary<string, bool> GetDeviceHealthStatus()
        {
            LogCommunication("Cihaz sağlık durumu sorgulanıyor...");

            Dictionary<string, bool> health = new Dictionary<string, bool>();

            // Cihaz durumunu al
            DeviceStatus status = QueryDeviceStatus();

            if (status != null)
            {
                health["IsReady"] = status.IsReady;
                health["IsRunning"] = status.IsRunning;
                health["EmergencyStop"] = !status.IsEmergencyStopped; // Tersini alıyoruz (sağlıklı = emergency yok)

                // Limit switch durumları
                bool[] switches = ReadLimitSwitches();
                if (switches != null)
                {
                    for (int i = 0; i < switches.Length; i++)
                    {
                        health[$"LimitSwitch_{i}"] = switches[i];
                    }
                }

                // Port durumu
                health["PortOpen"] = IsPortOpen();
                health["Connected"] = IsConnected;

                LogCommunication($"✓ Sağlık durumu: {health.Count} parametre okundu.");
            }
            else
            {
                LogCommunication("✗ Sağlık durumu sorgulaması başarısız!", true);
                health["Error"] = true;
            }

            return health;
        }

        private void OnDeviceStatusChanged(DeviceStatus status)
        {
            // Cihaz durumu değişikliği event tetikleme
        }



        private void HandleCommunicationError(Exception ex, string operation)
        {
            string msg = $"Hata ({operation}): {ex.Message}";

            // Hata geçmişine ekle
            lock (_errorLock)
            {
                _lastError = msg;

                if (_errorHistory.Count >= MAX_ERROR_HISTORY)
                {
                    _errorHistory.Dequeue(); // En eski hatayı sil
                }

                _errorHistory.Enqueue($"{DateTime.Now:HH:mm:ss} - {msg}");
            }

            LogCommunication(msg, true);
            OnErrorOccurred(msg, ErrorLevel.Error);

            // Eğer okuma/yazma hatası ise ve port koptuysa durumu bildir
            if ((operation == "ReadFromPort" || operation == "WriteToPort") && _serialPort != null && !_serialPort.IsOpen)
            {
                IsConnected = false;
                OnConnectionStatusChanged(false, CurrentPort);
            }
        }

        private void OnErrorOccurred(string errorMessage, ErrorLevel level)
        {
            ErrorOccurred?.Invoke(this, new ErrorEventArgs
            {
                ErrorMessage = errorMessage,
                Level = level,
                Timestamp = DateTime.Now
            });
        }

        public string GetLastError()
        {
            lock (_errorLock)
            {
                return _lastError;
            }
        }

        public void ClearErrors()
        {
            lock (_errorLock)
            {
                _errorHistory.Clear();
                _lastError = string.Empty;
                LogCommunication("Hata geçmişi temizlendi.");
            }
        }
        public List<string> GetErrorHistory()
        {
            lock (_errorLock)
            {
                return new List<string>(_errorHistory);
            }
        }
        private byte[] RetryCommand(byte commandCode, byte[] data, int maxRetries = 3, int timeoutMs = 1000)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                LogCommunication($"[RETRY] Deneme {attempt}/{maxRetries} - Komut 0x{commandCode:X2}");

                byte[] response = SendCommandAndWaitResponse(commandCode, data, timeoutMs);

                if (response != null)
                {
                    LogCommunication($"[RETRY] ✓ Başarılı! Deneme: {attempt}");
                    return response;
                }

                if (attempt < maxRetries)
                {
                    Thread.Sleep(200); // Denemeler arası bekleme
                }
            }

            LogCommunication($"[RETRY] ✗ {maxRetries} deneme sonunda başarısız!", true);
            return null;
        }



        private void OnConnectionStatusChanged(bool isConnected, string portName)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectionEventArgs
            {
                IsConnected = isConnected,
                PortName = portName,
                Timestamp = DateTime.Now
            });
        }

        private void OnCommandResponseReceived(byte commandCode, byte[] response)
        {
            CommandResponseReceived?.Invoke(this, new CommandResponseEventArgs
            {
                CommandCode = commandCode,
                Response = response,
                IsSuccess = true,
                Timestamp = DateTime.Now
            });
        }

        private byte[] ConvertDoubleToBytes(double value)
        {
            // Endianness (BigEndian/LittleEndian) cihazın işlemcisine göre değişir. 
            // Genellikle PC LittleEndian'dır. Cihaz da öyleyse direkt çeviririz.
            return BitConverter.GetBytes(value);
        }

        private double ConvertBytesToDouble(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 8)
            {
                LogCommunication("✗ ConvertBytesToDouble: Yetersiz veri!", true);
                return 0.0;
            }

            return BitConverter.ToDouble(bytes, 0);
        }

        private int ConvertBytesToInt(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
            {
                LogCommunication("✗ ConvertBytesToInt: Yetersiz veri!", true);
                return 0;
            }

            return BitConverter.ToInt32(bytes, 0);
        }
        private byte[] ConvertIntToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        private void LogCommunication(string message, bool isError = false)
        {
            string prefix = isError ? "[ERROR]" : "[INFO]";
            string logMsg = $"{DateTime.Now:HH:mm:ss} {prefix} {message}";

            // Debug penceresine yaz
            System.Diagnostics.Debug.WriteLine(logMsg);

            // ✅ YENİ: Event fırlat (MainForm dinleyecek)
            LogMessage?.Invoke(this, logMsg);
        }

        // Dispose metodunu da dolduralım ki sınıf kapanırken port açık kalmasın
        public void Dispose()
        {
            ClosePort();
            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }


    }

    #region Event Args Classes

    public class LoadCellDataEventArgs : EventArgs
    {
        public LoadCellDataPacket Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class DeviceStatusEventArgs : EventArgs
    {
        public DeviceStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
        public ErrorLevel Level { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string PortName { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CommandResponseEventArgs : EventArgs
    {
        public byte CommandCode { get; set; }
        public byte[] Response { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime Timestamp { get; set; }
    }

    #endregion

    #region Data Classes

    public class LoadCellDataPacket
    {
        public double RightHeel { get; set; }
        public double LeftHeel { get; set; }
        public double RightToe { get; set; }
        public double LeftToe { get; set; }
        public double WeightBalance { get; set; }
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class DeviceStatus
    {
        public bool IsReady { get; set; }
        public bool IsRunning { get; set; }
        public bool IsEmergencyStopped { get; set; }
        public bool[] ServoMotorStatus { get; set; } // 7 motor
        public bool[] StepMotorStatus { get; set; }  // 10 motor
        public bool[] LimitSwitchStatus { get; set; }
        public string ErrorCode { get; set; }
        public double CurrentSpeed { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum ErrorLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public enum CommandCode : byte
    {
        Connect = 0x01,
        Disconnect = 0x02,
        StartTherapy = 0x10,
        StopTherapy = 0x11,
        PauseTherapy = 0x12,
        ResumeTherapy = 0x13,
        EmergencyStop = 0x14,
        SetSpeed = 0x20,
        SetShoeSize = 0x21,
        SetSupportBar = 0x22,
        SetWeightReduction = 0x23,
        SetWinch = 0x24,
        GetSpeed = 0x25,
        ReadLoadCell = 0x30,
        ReadLimitSwitch = 0x31,
        ReadStatus = 0x32,
        SetServoMotor = 0x40,
        SetStepMotor = 0x41,
        GetMotorPosition = 0x42,
<<<<<<< HEAD
        GetServoPosition = 0x43,
        LoadPattern = 0x50,
        HomeDevice = 0x51,
        GetFirmwareVersion = 0x60,
        GetHealthStatus = 0x61,
=======
        GetServoPosition = 0x43, 
        LoadPattern = 0x50,
        HomeDevice = 0x51,
        GetFirmwareVersion = 0x60, 
        GetHealthStatus = 0x61,     
>>>>>>> 97da60aa201966cefec4af7dd41e7e826ba96776
    }

    #endregion
}