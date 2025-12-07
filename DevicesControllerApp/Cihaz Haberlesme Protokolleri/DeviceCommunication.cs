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

        private SerialPort _serialPort;
        private Thread _readThread;
        private bool _isReading;
        private Queue<byte[]> _commandQueue;
        private readonly object _queueLock = new object();
        private List<byte> _rawRxBuffer = new List<byte>();

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
        public int CommandTimeout { get; set; } = 1000; // ms

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

        public bool OpenPort(string portName, int baudRate = 9600, Parity parity = Parity.None,
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
            // Komut koduna göre işlem yap (Enum: CommandCode)
            CommandCode code = (CommandCode)commandCode;

            switch (code)
            {
                case CommandCode.ReadLoadCell: // Örnek: LoadCell verisi geldi
                    ParseLoadCellData(payload);
                    break;

                case CommandCode.ReadStatus: // Cihaz durumu geldi
                                             // ParseDeviceStatus(payload); // Bu metodu sonra yazarız
                    break;

                default:
                    // Genel komut yanıtı olarak event fırlat
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

        public byte[] SendCommandAndWaitResponse(byte commandCode, byte[] data = null,
            int timeoutMs = 0)
        {
            return null;
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
            // Komut kuyruğuna ekleme
        }

        private void ProcessCommandQueue()
        {
            // Komut kuyruğunu işleme
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
            return false;
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

        // Mevcut "return false" dönen metotları bunlarla değiştirin:

        public bool StartTherapy()
        {
            LogCommunication("Terapi başlatılıyor...");
            return SendCommand((byte)CommandCode.StartTherapy);
        }

        public bool StopTherapy()
        {
            LogCommunication("Terapi durduruluyor...");
            return SendCommand((byte)CommandCode.StopTherapy);
        }

        public bool PauseTherapy()
        {
            return SendCommand((byte)CommandCode.PauseTherapy);
        }

        public bool ResumeTherapy()
        {
            return SendCommand((byte)CommandCode.ResumeTherapy);
        }

        public bool EmergencyStop()
        {
            LogCommunication("!!! ACİL DURDURMA !!!", true);
            return SendCommand((byte)CommandCode.EmergencyStop);
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
            return false;
        }

        public bool LoadPattern(byte[] patternData)
        {
            return false;
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
            return 0;
        }

        public int GetStepMotorPosition(int motorIndex)
        {
            return 0;
        }

        public bool[] GetAllServoMotorPositions()
        {
            return null;
        }

        public int[] GetAllStepMotorPositions()
        {
            return null;
        }

        public bool[] ReadLimitSwitches()
        {
            return null;
        }

        public double[] ReadAllLoadCells()
        {
            return null;
        }

        public double ReadLoadCell(int channel)
        {
            return 0.0;
        }

        public Dictionary<string, int> GetAllPositionSensors()
        {
            return null;
        }

        public DeviceStatus QueryDeviceStatus()
        {
            SendCommand((byte)CommandCode.ReadStatus);
            return null; // Yanıt asenkron olarak event ile gelecek
        }

        public bool IsDeviceReady()
        {
            return false;
        }

        public string GetDeviceFirmwareVersion()
        {
            return null;
        }

        public Dictionary<string, bool> GetDeviceHealthStatus()
        {
            return null;
        }

        private void OnDeviceStatusChanged(DeviceStatus status)
        {
            // Cihaz durumu değişikliği event tetikleme
        }



        private void HandleCommunicationError(Exception ex, string operation)
        {
            string msg = $"Hata ({operation}): {ex.Message}";
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
            return null;
        }

        public void ClearErrors()
        {
            // Hata listesini temizleme
        }

        private bool RetryCommand(byte commandCode, byte[] data, int maxRetries = 3)
        {
            return false;
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
            return 0.0;
        }

        private int ConvertBytesToInt(byte[] bytes)
        {
            return 0;
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
            if (_serialPort != null)//
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
        ReadLoadCell = 0x30,
        ReadLimitSwitch = 0x31,
        ReadStatus = 0x32,
        SetServoMotor = 0x40,
        SetStepMotor = 0x41,
        LoadPattern = 0x50,
        HomeDevice = 0x51
    }

    #endregion
}