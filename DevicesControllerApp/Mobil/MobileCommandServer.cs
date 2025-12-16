using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
// Yeni DeviceCommunication namespace'ini ekliyoruz
using RehabilitationSystem.Communication;

namespace RehabilitationSystem.Mobile
{
    /// <summary>
    /// Mobil uygulamadan gelen TCP komutlarını dinler ve ana forma/cihaza iletir.
    /// </summary>
    public class MobileCommandServer : IDisposable
    {
        private readonly int _port;
        private readonly TherapySessionState _sessionState = new TherapySessionState();
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private bool _disposed;

        public event EventHandler<string> ClientConnected;
        public event EventHandler<string> ClientDisconnected;
        public event EventHandler<string> CommandProcessed;
        public event EventHandler<TherapySessionState> TherapyStateChanged;

        public bool IsRunning => _listener != null;

        public MobileCommandServer(int port = 9000)
        {
            _port = port;
        }

        public void Start()
        {
            if (_listener != null)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();

            // Arkaplanda dinleme döngüsünü başlat
            Task.Run(() => AcceptLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            if (_listener == null)
            {
                return;
            }

            try
            {
                _cts?.Cancel();
                _listener.Stop();
            }
            catch (Exception)
            {
                // Dinleme durdurulurken oluşabilecek hatalar sessizce yutulur.
            }
            finally
            {
                _listener = null;
            }
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _listener != null)
            {
                try
                {
                    if (!_listener.Pending())
                    {
                        await Task.Delay(150, token);
                        continue;
                    }

                    var client = await _listener.AcceptTcpClientAsync();
                    var endpoint = client.Client.RemoteEndPoint != null
                        ? client.Client.RemoteEndPoint.ToString()
                        : "bilinmiyor";

                    OnClientConnected(endpoint);
                    // Her istemciyi ayrı bir task'te işle
                    _ = Task.Run(() => HandleClientAsync(client, endpoint, token), token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnCommandProcessed($"Dinleme hatası: {ex.Message}");
                    await Task.Delay(500, token);
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, string endpoint, CancellationToken token)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, leaveOpen: true))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true })
            {
                try
                {
                    while (!token.IsCancellationRequested && client.Connected)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line == null)
                        {
                            break;
                        }

                        var trimmed = line.Trim();
                        if (string.IsNullOrWhiteSpace(trimmed))
                        {
                            continue;
                        }

                        var response = ProcessCommand(trimmed);
                        await writer.WriteLineAsync(response);
                    }
                }
                catch (Exception ex)
                {
                    OnCommandProcessed($"'{endpoint}' için okuma hatası: {ex.Message}");
                }
            }

            OnClientDisconnected(endpoint);
        }

        private string ProcessCommand(string command)
        {
            var lower = command.ToLowerInvariant();
            var payload = new { message = "Komut alındı" };
            var stateChanged = false;

            // Cihaz iletişim referansı
            var device = DeviceCommunication.Instance;

            try
            {
                switch (lower)
                {
                    case "ping":
                        payload = new { message = "Ana form aktif" };
                        break;
                    case "status":
                        payload = new { message = "Durum alındı" };
                        break;

                    // --- TERAPİ KONTROLLERİ ---
                    case "start":
                        if (device.StartTherapy()) // Cihaza gönder
                        {
                            _sessionState.Start();
                            stateChanged = true;
                            payload = new { message = "Terapi başlatıldı (Cihaz OK)" };
                        }
                        else
                        {
                            payload = new { message = "HATA: Cihaz başlatılamadı!" };
                        }
                        break;

                    case "stop":
                        if (device.StopTherapy()) // Cihaza gönder
                        {
                            _sessionState.Stop();
                            stateChanged = true;
                            payload = new { message = "Terapi durduruldu (Cihaz OK)" };
                        }
                        else
                        {
                            payload = new { message = "HATA: Cihaz durdurulamadı!" };
                        }
                        break;

                    case "pause":
                        if (device.PauseTherapy())
                        {
                            _sessionState.Pause();
                            stateChanged = true;
                            payload = new { message = "Terapi bekletildi" };
                        }
                        break;

                    case "resume":
                        if (device.ResumeTherapy())
                        {
                            _sessionState.Resume();
                            stateChanged = true;
                            payload = new { message = "Terapi devam ediyor" };
                        }
                        break;

                    case "emergencystop":
                        device.EmergencyStop(); // Kritik işlem, yanıt beklemeden state güncelle
                        _sessionState.Emergency();
                        stateChanged = true;
                        payload = new { message = "ACİL DURDURMA TETİKLENDİ!" };
                        break;

                    // --- VİNÇ VE MOTOR KONTROLLERİ ---
                    case "up": // Vinç Yukarı
                        if (device.SetWinchPosition(true))
                        {
                            _sessionState.MarkMovement("Vinc Yukari");
                            stateChanged = true;
                            payload = new { message = "Vinc yukari komutu iletildi" };
                        }
                        break;

                    case "down": // Vinç Aşağı
                        if (device.SetWinchPosition(false))
                        {
                            _sessionState.MarkMovement("Vinc Asagi");
                            stateChanged = true;
                            payload = new { message = "Vinc asagi komutu iletildi" };
                        }
                        break;

                    case "left":
                        // Sol motor (Örn: Motor Index 1)
                        // device.MoveMotor(1, 100); 
                        _sessionState.MarkMovement("Sola Hareket");
                        payload = new { message = "Sola hareket komutu alındı" };
                        break;

                    case "right":
                        // Sağ motor (Örn: Motor Index 1, -100)
                        // device.MoveMotor(1, -100);
                        _sessionState.MarkMovement("Sağa Hareket");
                        payload = new { message = "Sağa hareket komutu alındı" };
                        break;

                    // --- AYAR KONTROLLERİ ---
                    case "footincrease":
                        _sessionState.IncreaseShoeSize();
                        device.SetShoeSize(_sessionState.ShoeSize); // Cihaza yeni numarayı gönder
                        stateChanged = true;
                        payload = new { message = $"Ayak numarası büyütüldü: {_sessionState.ShoeSize}" };
                        break;

                    case "footdecrease":
                        _sessionState.DecreaseShoeSize();
                        device.SetShoeSize(_sessionState.ShoeSize); // Cihaza yeni numarayı gönder
                        stateChanged = true;
                        payload = new { message = $"Ayak numarası küçültüldü: {_sessionState.ShoeSize}" };
                        break;

                    case "barup":
                        _sessionState.MoveSupportBar(true);
                        device.SetSupportBarHeight(_sessionState.SupportBarHeight); // Cihaza gönder
                        stateChanged = true;
                        payload = new { message = "Destek barı yükseltildi" };
                        break;

                    case "bardown":
                        _sessionState.MoveSupportBar(false);
                        device.SetSupportBarHeight(_sessionState.SupportBarHeight); // Cihaza gönder
                        stateChanged = true;
                        payload = new { message = "Destek barı alçaltıldı" };
                        break;

                    case "weightincrease":
                        _sessionState.AdjustWeight(true);
                        device.SetWeightReduction(_sessionState.WeightSupport); // Cihaza gönder
                        stateChanged = true;
                        payload = new { message = "Ağırlık azaltma artırıldı" };
                        break;

                    case "weightdecrease":
                        _sessionState.AdjustWeight(false);
                        device.SetWeightReduction(_sessionState.WeightSupport); // Cihaza gönder
                        stateChanged = true;
                        payload = new { message = "Ağırlık azaltma düşürüldü" };
                        break;

                    case "disconnect":
                        _sessionState.Stop();
                        // Mobil bağlantıyı keserken cihazı durdurmak isteyebiliriz:
                        // device.StopTherapy(); 
                        stateChanged = true;
                        payload = new { message = "Bağlantı kesildi" };
                        OnClientDisconnected("disconnect");
                        break;

                    default:
                        return _serializer.Serialize(new
                        {
                            status = "error",
                            message = "Bilinmeyen komut",
                            received = command
                        });
                }

                // Mobil uygulamaya JSON formatında yanıt dönüyoruz
                var response = _serializer.Serialize(new
                {
                    status = "ok",
                    command = lower,
                    data = payload,
                    therapy = _sessionState.ToTransportModel(), // Güncel durumu da gönder
                    timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
                });

                OnCommandProcessed($"Komut: {lower}");

                if (stateChanged)
                {
                    OnTherapyStateChanged();
                }

                return response;
            }
            catch (Exception ex)
            {
                return _serializer.Serialize(new
                {
                    status = "error",
                    message = "Komut işlenemedi",
                    detail = ex.Message
                });
            }
        }

        private void OnClientConnected(string endpoint)
        {
            var handler = ClientConnected;
            if (handler != null)
            {
                handler(this, endpoint);
            }
        }

        private void OnClientDisconnected(string endpoint)
        {
            var handler = ClientDisconnected;
            if (handler != null)
            {
                handler(this, endpoint);
            }
        }

        private void OnCommandProcessed(string message)
        {
            var handler = CommandProcessed;
            if (handler != null)
            {
                handler(this, message);
            }
        }

        private void OnTherapyStateChanged()
        {
            var handler = TherapyStateChanged;
            if (handler != null)
            {
                handler(this, _sessionState);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Stop();
            _cts?.Dispose();
        }
    }

    // Mobil uygulama için durum (state) tutan yardımcı sınıf
    public class TherapySessionState
    {
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        public string PatientName { get; set; } = "Mobil Hasta";
        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }
        public bool IsEmergency { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int TargetDurationMinutes { get; set; } = 30;
        public double WeightSupport { get; set; } = 20;
        public int ShoeSize { get; set; } = 42;
        public double SupportBarHeight { get; set; } = 0.4;
        public string LastCommand { get; set; } = "hazir";
        public string StatusText { get; set; } = "Hazir";
        public int CommandSerial { get; set; }

        public void Start()
        {
            IsRunning = true;
            IsPaused = false;
            IsEmergency = false;
            StartedAt = DateTime.UtcNow;
            Touch("start");
        }

        public void Stop()
        {
            IsRunning = false;
            IsPaused = false;
            IsEmergency = false;
            Touch("stop");
        }

        public void Pause()
        {
            if (!IsRunning)
            {
                return;
            }

            IsPaused = true;
            Touch("pause");
        }

        public void Resume()
        {
            if (!IsRunning)
            {
                return;
            }

            IsPaused = false;
            Touch("resume");
        }

        public void Emergency()
        {
            IsEmergency = true;
            IsRunning = false;
            IsPaused = false;
            Touch("emergencystop");
        }

        public void IncreaseShoeSize()
        {
            ShoeSize = Math.Min(50, ShoeSize + 1);
            Touch("footincrease");
        }

        public void DecreaseShoeSize()
        {
            ShoeSize = Math.Max(30, ShoeSize - 1);
            Touch("footdecrease");
        }

        public void MoveSupportBar(bool up)
        {
            var delta = up ? 0.01 : -0.01;
            SupportBarHeight = Math.Round(Math.Max(0, SupportBarHeight + delta), 2);
            Touch(up ? "barup" : "bardown");
        }

        public void AdjustWeight(bool increase)
        {
            var delta = increase ? 1 : -1;
            WeightSupport = Math.Max(0, WeightSupport + delta);
            Touch(increase ? "weightincrease" : "weightdecrease");
        }

        public void MarkMovement(string description)
        {
            Touch(description);
        }

        public object ToTransportModel()
        {
            return new
            {
                patientName = PatientName,
                isRunning = IsRunning,
                isPaused = IsPaused,
                isEmergency = IsEmergency,
                startedAt = StartedAt.HasValue ? StartedAt.Value.ToString("o", _culture) : null,
                lastUpdate = LastUpdate.HasValue ? LastUpdate.Value.ToString("o", _culture) : null,
                targetDurationMinutes = TargetDurationMinutes,
                weightSupport = WeightSupport,
                shoeSize = ShoeSize,
                supportBarHeight = SupportBarHeight,
                lastCommand = LastCommand,
                statusText = BuildStatusText(),
                commandSerial = CommandSerial
            };
        }

        private void Touch(string command)
        {
            LastCommand = command;
            LastUpdate = DateTime.UtcNow;
            CommandSerial++;
            StatusText = BuildStatusText();
        }

        private string BuildStatusText()
        {
            if (IsEmergency)
            {
                return "Acil durdurma aktif";
            }

            if (IsRunning && IsPaused)
            {
                return "Beklemede";
            }

            if (IsRunning)
            {
                return "Devam ediyor";
            }

            return "Hazir";
        }
    }
}