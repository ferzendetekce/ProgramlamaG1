using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RehabilitationSystem.EngineAPI.Services
{
    /// <summary>
    /// Windows uygulamasındaki ana motor ile asenkron TCP soket iletişimi kuran servis.
    /// Bağlantıyı yeniden kullanarak performansı artırır.
    /// </summary>
    public static class Engine
    {
        private static readonly string engineIp = "127.0.0.1"; // Ana makinenin IP adresi
        private static readonly int enginePort = 9000; // Ana makinenin portu

        private static TcpClient? client;
        private static readonly object connectionLock = new object();

        /// <summary>
        /// TCP istemcisini ana makineye asenkron olarak bağlar.
        /// Bağlantı zaten varsa yeniden oluşturmaz, kopmuşsa yeniden kurar.
        /// </summary>
        private static async Task ConnectAsync()
        {
            // 'lock' anahtar kelimesi ile aynı anda birden fazla thread'in bağlantı kurmasını engelle.
            lock (connectionLock)
            {
                if (client != null && client.Connected)
                {
                    return; // Zaten bağlıysa bir şey yapma.
                }

                // Eğer bağlantı kopmuş veya hiç kurulmamışsa, eskisini temizle.
                client?.Dispose();
                client = new TcpClient();
            }

            try
            {
                await client.ConnectAsync(engineIp, enginePort);
                Console.WriteLine("Ana makineye başarıyla bağlandı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bağlantı hatası: {ex.Message}");
                client?.Dispose();
                client = null;
                throw; // Hatanın yukarıya bildirilmesi önemli.
            }
        }

        /// <summary>
        /// Ana makineye belirtilen komutu asenkron olarak gönderir.
        /// </summary>
        private static async Task SendCommandAsync(string cmd)
        {
            try
            {
                await ConnectAsync();
                if (client == null)
                    return;

                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(cmd);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine($"Komut gönderildi: {cmd}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Komut gönderilemedi: {ex.Message}");
                // Hata durumunda istemciyi temizle ki bir sonraki denemede yeniden bağlansın.
                client?.Dispose();
                client = null;
            }
        }

        // Asenkron Komut Metodları ('speed' parametresi kaldırıldı)
        public static async Task MoveUp() => await SendCommandAsync("up");

        public static async Task MoveDown() => await SendCommandAsync("down");

        public static async Task MoveLeft() => await SendCommandAsync("left");

        public static async Task MoveRight() => await SendCommandAsync("right");

        public static async Task Start() => await SendCommandAsync("start");

        public static async Task Stop() => await SendCommandAsync("stop");

        public static async Task Pause() => await SendCommandAsync("pause");

        public static async Task Resume() => await SendCommandAsync("resume");

        public static async Task EmergencyStop() => await SendCommandAsync("emergencystop");

        public static async Task FootIncrease() => await SendCommandAsync("footincrease");

        public static async Task FootDecrease() => await SendCommandAsync("footdecrease");

        public static async Task BarUp() => await SendCommandAsync("barup");

        public static async Task BarDown() => await SendCommandAsync("bardown");

        public static async Task WeightIncrease() => await SendCommandAsync("weightincrease");

        public static async Task WeightDecrease() => await SendCommandAsync("weightdecrease");
    }
}
