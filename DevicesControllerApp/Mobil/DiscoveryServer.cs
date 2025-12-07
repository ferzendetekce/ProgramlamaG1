using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RehabilitationSystem.Mobile
{
    /// <summary>
    /// Mobil uygulamanın otomatik keşif isteklerine UDP ile yanıt verir.
    /// </summary>
    internal class DiscoveryServer : IDisposable
    {
        private readonly int _listenPort;
        private readonly int _apiPort;
        private UdpClient _udpClient;
        private CancellationTokenSource _cts;
        private Task _loopTask;
        private bool _disposed;

        public bool IsRunning => _loopTask != null && !_loopTask.IsCompleted;

        public DiscoveryServer(int listenPort, int apiPort)
        {
            _listenPort = listenPort;
            _apiPort = apiPort;
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            _udpClient = new UdpClient(_listenPort);
            _loopTask = Task.Run(() => ListenLoop(_cts.Token));
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                _cts.Cancel();
                _udpClient.Close();
                _loopTask.Wait(1500);
            }
            catch
            {
                // Sessiz geç.
            }
        }

        private async Task ListenLoop(CancellationToken token)
        {
            var serializer = new JavaScriptSerializer();
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync();
                    var message = Encoding.UTF8.GetString(result.Buffer);
                    if (!string.Equals(message, "DISCOVER_REHAB", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var payload = serializer.Serialize(new
                    {
                        ip = GetLocalIPv4() ?? "127.0.0.1",
                        port = _apiPort,
                        name = "RehabEngineAPI"
                    });

                    var bytes = Encoding.UTF8.GetBytes(payload);
                    await _udpClient.SendAsync(bytes, bytes.Length, result.RemoteEndPoint);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch
                {
                    await Task.Delay(200, token);
                }
            }
        }

        private static string GetLocalIPv4()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(x));
            return ip?.ToString();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Stop();
            _udpClient?.Dispose();
            _cts?.Dispose();
            _disposed = true;
        }
    }
}
