using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using RehabilitationSystem.Communication;

namespace RehabilitationSystem.Mobile
{
    public class MobileCommandServer : IDisposable
    {
        private readonly int _port;
        private readonly TherapySessionState _sessionState = new TherapySessionState();
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private bool _disposed;

        // Olaylar (Events)
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
            if (_listener != null) return;

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _port); // Loopback yerine Any yaptık ki dışardan bağlanabilsin
            _listener.Start();

            Task.Run(() => AcceptLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            if (_listener == null) return;
            try
            {
                _cts?.Cancel();
                _listener.Stop();
            }
            catch (Exception) { }
            finally { _listener = null; }
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
                    var endpoint = client.Client.RemoteEndPoint?.ToString() ?? "bilinmiyor";
                    OnClientConnected(endpoint);
                    _ = Task.Run(() => HandleClientAsync(client, endpoint, token), token);
                }
                catch { break; }
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
                        if (line == null) break;

                        var trimmed = line.Trim();
                        if (string.IsNullOrWhiteSpace(trimmed)) continue;

                        var response = ProcessCommand(trimmed);
                        await writer.WriteLineAsync(response);
                    }
                }
                catch (Exception ex)
                {
                    OnCommandProcessed($"'{endpoint}' hatası: {ex.Message}");
                }
            }
            OnClientDisconnected(endpoint);
        }

        // --- GÜNCELLENEN KISIM: YENİ DEVICECOMMUNICATION İLE ENTEGRASYON ---
        private string ProcessCommand(string command)
        {
            var lower = command.ToLowerInvariant();
            object payload = new { message = "İşlem başarılı" };
            bool stateChanged = false;
            string status = "ok";
            string errorMessage = "";

            // Singleton instance'a erişim
            var device = DeviceCommunication.Instance;

            // ÖNEMLİ: Cihaz bağlı mı kontrolü (Ping ve Connect komutları hariç)
            bool deviceConnected = device.IsConnected;

            try
            {
                bool hwResult = true; 

                switch (lower)
                {
                    case "ping":
                        // Bağlantı kontrolü için kullanılır, cihaz bağlı olmasa da cevap vermeli
                        payload = new { 
                            message = "Sunucu aktif", 
                            deviceConnected = deviceConnected,
                            port = device.CurrentPort 
                        };
                        break;

                    case "status":
                        // Durum sorgusu
                        payload = new { 
                            message = "Durum alındı",
                            deviceConnected = deviceConnected 
                        };
                        break;

                    case "start":
                        if (!deviceConnected) errorMessage = "Cihaz bilgisayara bağlı değil (Port kapalı).";
                        else 
                        {
                            hwResult = device.StartTherapy();
                            if (hwResult) {
                                _sessionState.Start();
                                stateChanged = true;
                                payload = new { message = "Terapi başlatıldı" };
                            } else errorMessage = "Cihaz başlatılamadı (Komut başarısız).";
                        }
                        break;

                    case "stop":
                        // Durdurma işlemi bağlantı kopsa bile arayüzü durdurmalı
                        if (deviceConnected) device.StopTherapy();
                        _sessionState.Stop();
                        stateChanged = true;
                        payload = new { message = "Terapi durduruldu" };
                        break;

                    case "pause":
                        if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                        else 
                        {
                            hwResult = device.PauseTherapy();
                            if (hwResult) {
                                _sessionState.Pause();
                                stateChanged = true;
                                payload = new { message = "Terapi bekletildi" };
                            } else errorMessage = "Cihaz yanıt vermedi.";
                        }
                        break;

                    case "resume":
                        if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                        else 
                        {
                            hwResult = device.ResumeTherapy();
                            if (hwResult) {
                                _sessionState.Resume();
                                stateChanged = true;
                                payload = new { message = "Terapi devam ediyor" };
                            } else errorMessage = "Cihaz yanıt vermedi.";
                        }
                        break;

                    case "emergencystop":
                        if (deviceConnected) device.EmergencyStop();
                        _sessionState.Emergency();
                        stateChanged = true;
                        payload = new { message = "ACİL DURDURMA!" };
                        break;

                    // --- Manuel Hareketler ---
                    case "up":
                        // DİKKAT: Grup 2 SetWinchPosition metodunu "return false" bırakmış.
                        // Bu yüzden hata vermemesi için sadece simüle ediyoruz veya uyarıyoruz.
                        if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                        else 
                        {
                             // Grup 2 kodunu düzeltirse burası çalışır:
                            hwResult = device.SetWinchPosition(true); 
                            
                            // Geçici çözüm: Kodları henüz yazılmadığı için hata fırlatmayalım:
                            // hwResult = true; 
                            
                            if (hwResult) {
                                _sessionState.MarkMovement("Vinc Yukari");
                                payload = new { message = "Vinç yukarı" };
                            } else errorMessage = "Vinç kontrolü henüz aktif değil.";
                        }
                        break;

                    case "down":
                        if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                        else 
                        {
                            hwResult = device.SetWinchPosition(false);
                            if (hwResult) {
                                _sessionState.MarkMovement("Vinc Asagi");
                                payload = new { message = "Vinç aşağı" };
                            } else errorMessage = "Vinç kontrolü henüz aktif değil.";
                        }
                        break;

                    case "footincrease":
                        if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                        else
                        {
                            hwResult = device.SetShoeSize(_sessionState.ShoeSize + 1);
                            if (hwResult) {
                                _sessionState.IncreaseShoeSize();
                                stateChanged = true;
                                payload = new { message = "Ayak no büyütüldü" };
                            } else errorMessage = "Ayak ayarı yapılamadı.";
                        }
                        break;

                    case "footdecrease":
                         if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                         else
                         {
                            hwResult = device.SetShoeSize(_sessionState.ShoeSize - 1);
                            if (hwResult) {
                                _sessionState.DecreaseShoeSize();
                                stateChanged = true;
                                payload = new { message = "Ayak no küçültüldü" };
                            } else errorMessage = "Ayak ayarı yapılamadı.";
                         }
                        break;

                    case "barup":
                        if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                        else
                        {
                            hwResult = device.SetSupportBarHeight(_sessionState.SupportBarHeight + 0.01);
                            if (hwResult) {
                                _sessionState.MoveSupportBar(true);
                                stateChanged = true;
                                payload = new { message = "Bar yükseltildi" };
                            } else errorMessage = "Bar ayarı yapılamadı.";
                        }
                        break;

                    case "bardown":
                         if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                         else
                         {
                            hwResult = device.SetSupportBarHeight(_sessionState.SupportBarHeight - 0.01);
                            if (hwResult) {
                                _sessionState.MoveSupportBar(false);
                                stateChanged = true;
                                payload = new { message = "Bar alçaltıldı" };
                            } else errorMessage = "Bar ayarı yapılamadı.";
                         }
                        break;

                    case "weightincrease":
                         if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                         else
                         {
                            hwResult = device.SetWeightReduction(_sessionState.WeightSupport + 1);
                            if (hwResult) {
                                _sessionState.AdjustWeight(true);
                                stateChanged = true;
                                payload = new { message = "Ağırlık desteği artırıldı" };
                            } else errorMessage = "Ağırlık ayarı yapılamadı.";
                         }
                        break;

                    case "weightdecrease":
                         if (!deviceConnected) errorMessage = "Cihaz bağlı değil.";
                         else
                         {
                            hwResult = device.SetWeightReduction(_sessionState.WeightSupport - 1);
                            if (hwResult) {
                                _sessionState.AdjustWeight(false);
                                stateChanged = true;
                                payload = new { message = "Ağırlık desteği azaltıldı" };
                            } else errorMessage = "Ağırlık ayarı yapılamadı.";
                         }
                        break;

                    case "disconnect":
                        // Mobil uygulama bağlantıyı kestiğinde
                        if (deviceConnected) device.StopTherapy();
                        _sessionState.Stop();
                        stateChanged = true;
                        payload = new { message = "Mobil bağlantı sonlandırıldı" };
                        OnClientDisconnected("disconnect");
                        break;

                    default:
                        status = "error";
                        errorMessage = "Bilinmeyen komut";
                        break;
                }

                // Hata mesajı varsa status error olur
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    status = "error";
                    payload = new { message = errorMessage };
                }

                var finalResponse = _serializer.Serialize(new
                {
                    status = status,
                    command = lower,
                    data = payload,
                    therapy = _sessionState.ToTransportModel(),
                    timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
                });

                OnCommandProcessed($"Komut: {lower} | Durum: {status}");
                if (stateChanged) OnTherapyStateChanged();

                return finalResponse;
            }
            catch (Exception ex)
            {
                return _serializer.Serialize(new
                {
                    status = "error",
                    message = "Sunucu hatası",
                    detail = ex.Message
                });
            }
        }

        private void OnClientConnected(string endpoint) => ClientConnected?.Invoke(this, endpoint);
        private void OnClientDisconnected(string endpoint) => ClientDisconnected?.Invoke(this, endpoint);
        private void OnCommandProcessed(string message) => CommandProcessed?.Invoke(this, message);
        private void OnTherapyStateChanged() => TherapyStateChanged?.Invoke(this, _sessionState);

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Stop();
            _cts?.Dispose();
        }
    }
}