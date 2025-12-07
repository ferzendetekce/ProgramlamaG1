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

            LogToScreen("TEST MODU AKTİF. Lütfen 'Buton10' ile bağlanın, 'Terapi' ile komut gönderin.");
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

            DeviceCommunication.Instance.LogMessage += (s, msg) =>
            {
                this.Invoke((MethodInvoker)delegate {
                    LogToScreen(msg);
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
                LogToScreen("Bağlantı yok! Önce 'Buton10' butonuna basarak bağlanın.");
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
            LogToScreen("=== PORT SEÇİMİ ===");
            LogToScreen("Portlar taranıyor...");

            string[] ports = DeviceCommunication.Instance.GetAvailablePorts();

            if (ports.Length == 0)
            {
                LogToScreen("HİÇBİR PORT BULUNAMADI! (USB kablosunu veya sanal portu kontrol edin)");
                MessageBox.Show("Sistemde hiçbir seri port bulunamadı!\n\nLütfen:\n- USB kablosunu kontrol edin\n- Sanal port programını çalıştırın",
                    "Port Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bulunan portları listele
            LogToScreen($"{ports.Length} adet port bulundu:");
            foreach (string port in ports)
            {
                LogToScreen($"  - {port}");
            }

            // Kullanıcıya seçim yaptır
            string selectedPort = ShowPortSelectionDialog(ports);

            if (!string.IsNullOrEmpty(selectedPort))
            {
                LogToScreen($"{selectedPort} portuna bağlanılıyor...");

                bool result = DeviceCommunication.Instance.OpenPort(selectedPort);

                if (result)
                {
                    LogToScreen($"✓ {selectedPort} portuna başarıyla bağlandı!");
                }
                else
                {
                    LogToScreen($"✗ {selectedPort} portuna bağlanılamadı!");
                }
            }
            else
            {
                LogToScreen("Port seçimi iptal edildi.");
            }

            // Settings ekranını aç
            Settings s = new Settings();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        // Port seçim dialogu
        private string ShowPortSelectionDialog(string[] ports)
        {
            Form portDialog = new Form();
            portDialog.Text = "Seri Port Seçin";
            portDialog.Size = new Size(400, 300);
            portDialog.StartPosition = FormStartPosition.CenterParent;
            portDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            portDialog.MaximizeBox = false;
            portDialog.MinimizeBox = false;

            // Label
            Label lblInfo = new Label();
            lblInfo.Text = "Bağlanmak istediğiniz portu seçin:";
            lblInfo.Location = new Point(20, 20);
            lblInfo.Size = new Size(350, 20);
            portDialog.Controls.Add(lblInfo);

            // ListBox (Port listesi)
            ListBox lstPorts = new ListBox();
            lstPorts.Location = new Point(20, 50);
            lstPorts.Size = new Size(350, 150);
            lstPorts.Font = new Font("Consolas", 10, FontStyle.Bold);

            foreach (string port in ports)
            {
                lstPorts.Items.Add(port);
            }

            // İlk portu otomatik seç
            if (lstPorts.Items.Count > 0)
                lstPorts.SelectedIndex = 0;

            portDialog.Controls.Add(lstPorts);

            // Bağlan butonu
            Button btnConnect = new Button();
            btnConnect.Text = "Bağlan";
            btnConnect.Location = new Point(190, 220);
            btnConnect.Size = new Size(90, 30);
            btnConnect.DialogResult = DialogResult.OK;
            portDialog.Controls.Add(btnConnect);

            // İptal butonu
            Button btnCancel = new Button();
            btnCancel.Text = "İptal";
            btnCancel.Location = new Point(290, 220);
            btnCancel.Size = new Size(80, 30);
            btnCancel.DialogResult = DialogResult.Cancel;
            portDialog.Controls.Add(btnCancel);

            // Çift tıklamayla da seçim yapılabilsin
            lstPorts.DoubleClick += (s, e) => {
                if (lstPorts.SelectedItem != null)
                {
                    portDialog.DialogResult = DialogResult.OK;
                    portDialog.Close();
                }
            };

            // Dialog'u göster
            portDialog.AcceptButton = btnConnect;
            portDialog.CancelButton = btnCancel;

            string selectedPort = null;

            if (portDialog.ShowDialog() == DialogResult.OK && lstPorts.SelectedItem != null)
            {
                selectedPort = lstPorts.SelectedItem.ToString();
            }

            portDialog.Dispose();
            return selectedPort;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            LogToScreen("=== CRC HESAPLAMA TESTİ ===");

            // LoadCell paketi (CRC'siz kısım)
            byte[] testData = new byte[]
            {
        0x15, 0x30, // Length + Command
        0x00, 0x00, 0xCC, 0x41, // 25.5f (RightHeel)
        0x00, 0x00, 0xF0, 0x41, // 30.0f (RightToe)
        0x00, 0x00, 0xE0, 0x41, // 28.0f (LeftHeel)
        0x00, 0x00, 0xD6, 0x41, // 26.75f (LeftToe)
        0x7B, 0x00, 0x00, 0x00  // 123 (Index)
            };

            ushort crc = DeviceCommunication.Instance.CalculateCRC16(testData);

            LogToScreen($"Hesaplanan CRC: 0x{crc:X4}");
            LogToScreen($"CRC Low Byte: 0x{(byte)(crc & 0xFF):X2}");
            LogToScreen($"CRC High Byte: 0x{(byte)(crc >> 8):X2}");

            // TAM PAKETİ OLUŞTUR
            List<byte> fullPacket = new List<byte>();
            fullPacket.Add(0x55); // Header 1
            fullPacket.Add(0xAA); // Header 2
            fullPacket.AddRange(testData);
            fullPacket.Add((byte)(crc & 0xFF));        // CRC Low
            fullPacket.Add((byte)((crc >> 8) & 0xFF)); // CRC High

            string hexPacket = string.Join(" ", fullPacket.Select(b => b.ToString("X2")));
            LogToScreen($"TAM PAKET (Hercules'e yapıştır):");
            LogToScreen(hexPacket);
        }
    }
}