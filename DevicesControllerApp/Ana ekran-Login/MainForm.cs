using DevicesControllerApp.Ana_ekran_Login;
using DevicesControllerApp.Ayarlar;
using DevicesControllerApp.Hasta_kayit;
using DevicesControllerApp.Kullanici;
using DevicesControllerApp.Raporlama;
using DevicesControllerApp.Servis;
using DevicesControllerApp.Terapi;
using DevicesControllerApp.Veri_izleme;
using RehabilitationSystem.Communication; // Haberleşme Kütüphanesi
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevicesControllerApp
{
    public partial class MainForm : Form
    {
        // Test amaçlı, kod ile oluşturulan Log Kutusu
        private ListBox _debugLogBox;

        public MainForm()
        {
            InitializeComponent();

            // 1. Orijinal Login İşlemi
            Login login = new Login();
            login.ShowDialog();
            this.Text = login.username;

            // 2. Test Ortamını Hazırla (Designer'a dokunmadan)
            PrepareTestEnvironment();
        }

        // --- TEST ORTAMI KURULUMU (SADECE KOD İLE) ---
        private void PrepareTestEnvironment()
        {
            // A) Ekrana geçici bir Log kutusu ekle
            _debugLogBox = new ListBox();
            _debugLogBox.Dock = DockStyle.Bottom; // Ekranın altına yapış
            _debugLogBox.Height = 150;            // Yüksekliği 150px olsun
            _debugLogBox.BackColor = Color.Black;
            _debugLogBox.ForeColor = Color.Lime;  // Matrix yeşili :)
            _debugLogBox.Font = new Font("Consolas", 9);

            // Kontrolü forma ekle ve en öne getir
            this.Controls.Add(_debugLogBox);
            _debugLogBox.BringToFront();

            // B) Haberleşme Eventlerini Dinle (Veri Alma ve Buffer Testi)
            InitializeCommunicationEvents();

            LogToScreen("TEST MODU AKTİF. Lütfen 'Ayarlar' ile bağlanın, 'Terapi' ile komut gönderin.");
        }

        private void InitializeCommunicationEvents()
        {
            // Bağlantı durumu değişince
            DeviceCommunication.Instance.ConnectionStatusChanged += (s, e) =>
            {
                this.Invoke((MethodInvoker)delegate {
                    string durum = e.IsConnected ? "BAĞLANDI" : "KOPTU";
                    LogToScreen($"[BAĞLANTI] {e.PortName} -> {durum}");
                });
            };

            // Veri gelince (Buffer'dan okuma testi)
            // DeviceCommunication sınıfı veriyi alıp buffer'a atıyor ve bu eventi fırlatıyor.
            DeviceCommunication.Instance.LoadCellDataReceived += (s, e) =>
            {
                this.Invoke((MethodInvoker)delegate {
                    // Buffer'daki son veriyi çekip ekrana basalım
                    var latestData = DeviceCommunication.Instance.GetLatestLoadCellData();
                    if (latestData != null)
                    {
                        // Burası çok hızlı akar, testte aktığını görmek yeterli
                         LogToScreen($"[VERİ ALINDI] Denge: %{latestData.WeightBalance:F2}"); 
                    }
                });
            };

            // Hata olunca
            DeviceCommunication.Instance.ErrorOccurred += (s, e) =>
            {
                this.Invoke((MethodInvoker)delegate {
                    LogToScreen($"[HATA] {e.ErrorMessage}");
                });
            };

            // Komut cevabı gelince
            DeviceCommunication.Instance.CommandResponseReceived += (s, e) =>
            {
                this.Invoke((MethodInvoker)delegate {
                    LogToScreen($"[CEVAP] Cihazdan yanıt geldi. Kod: {e.CommandCode}");
                });
            };
        }

        // Log yazdırma yardımcısı
        private void LogToScreen(string msg)
        {
            if (_debugLogBox != null && !_debugLogBox.IsDisposed)
            {
                if (_debugLogBox.Items.Count > 100) _debugLogBox.Items.RemoveAt(0);
                _debugLogBox.Items.Add($"{DateTime.Now:HH:mm:ss} > {msg}");
                _debugLogBox.TopIndex = _debugLogBox.Items.Count - 1; // Otomatik kaydır
            }
        }

        // --- MEVCUT BUTONLARIN İÇİNE TEST KODU GÖMME ---

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // >>> TEST: BAĞLANTI AÇMA <<<
            // Otomatik olarak mevcut portları tarayıp ilkine bağlanmayı dener.
            LogToScreen("Portlar taranıyor...");
            string[] ports = DeviceCommunication.Instance.GetAvailablePorts();

            if (ports.Length > 0)
            {
                string targetPort = ports[0];
                LogToScreen($"{targetPort} portuna bağlanılıyor...");
                DeviceCommunication.Instance.OpenPort(targetPort);
            }
            else
            {
                LogToScreen("HİÇBİR PORT BULUNAMADI! (USB kablosunu veya sanal portu kontrol edin)");
            }
            // >>> TEST SONU <<<

            Settings s = new Settings();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnTherapy_Click(object sender, EventArgs e)
        {
            // >>> TEST: KOMUT GÖNDERME <<<
            if (DeviceCommunication.Instance.IsConnected)
            {
                LogToScreen("'Terapi Başlat' komutu gönderiliyor...");
                bool result = DeviceCommunication.Instance.StartTherapy();

                if (result) LogToScreen("Komut başarıyla porta yazıldı.");
                else LogToScreen("Komut gönderme başarısız!");
            }
            else
            {
                LogToScreen("Bağlantı yok! Önce 'Ayarlar' butonuna basarak bağlanın.");
            }
            // >>> TEST SONU <<<

            Therapy s = new Therapy();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnMonitoring_Click(object sender, EventArgs e)
        {
            // >>> TEST: ANLIK BUFFER KONTROLÜ <<<
            // Bu butona bastığınızda o an bufferda kaç veri var görebilirsiniz
            int count = DeviceCommunication.Instance.GetLoadCellBuffer(100).Count;
            LogToScreen($"Buffer Durumu: Şuan hafızada {count} adet veri paketi var.");
            // >>> TEST SONU <<<

            DataMonitoring s = new DataMonitoring();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        // Diğer butonlar standart işlevlerine devam eder
        private void btnPatient_Click(object sender, EventArgs e)
        {
            PatientRegistration s = new PatientRegistration();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            Reports s = new Reports();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnService_Click(object sender, EventArgs e)
        {
            Service s = new Service();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            UserRegistration s = new UserRegistration();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Boş
        }

        // Form kapanırken bağlantıyı temizle
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DeviceCommunication.Instance.Dispose();
            base.OnFormClosing(e);
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            LogToScreen("=== LOOPBACK TEST BAŞLIYOR ===");

            // FAKE LoadCell verisi oluştur
            byte[] fakeLoadCellPayload = new byte[20];

            // RightHeel = 25.5 kg
            BitConverter.GetBytes(25.5f).CopyTo(fakeLoadCellPayload, 0);
            // RightToe = 30.2 kg
            BitConverter.GetBytes(30.2f).CopyTo(fakeLoadCellPayload, 4);
            // LeftHeel = 28.1 kg
            BitConverter.GetBytes(28.1f).CopyTo(fakeLoadCellPayload, 8);
            // LeftToe = 26.8 kg
            BitConverter.GetBytes(26.8f).CopyTo(fakeLoadCellPayload, 12);
            // Index = 123
            BitConverter.GetBytes(123).CopyTo(fakeLoadCellPayload, 16);

            // ParseLoadCellData'yı direkt çağır (simulate data received)
            var comm = DeviceCommunication.Instance;
            var parseMethod = typeof(DeviceCommunication).GetMethod("ParseLoadCellData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            parseMethod.Invoke(comm, new object[] { fakeLoadCellPayload });

            LogToScreen("Fake veri buffer'a eklendi. Buffer'ı kontrol et!");
        }

      
    }
}