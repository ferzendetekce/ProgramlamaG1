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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevicesControllerApp
{
    public partial class MainForm : Form
    {
        // Test amaçlı, kod ile oluşturulan Log Kutusu
        private ListBox _debugLogBox;
        private System.Windows.Forms.Timer _dataTimer;
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
            // Log kutusu oluştur
            _debugLogBox = new ListBox();
            _debugLogBox.Dock = DockStyle.Bottom;
            _debugLogBox.Height = 150;
            _debugLogBox.BackColor = Color.Black;
            _debugLogBox.ForeColor = Color.Lime;
            _debugLogBox.Font = new Font("Consolas", 9);

            this.Controls.Add(_debugLogBox);
            _debugLogBox.BringToFront();

            InitializeCommunicationEvents();

            // ✅ GERÇEK CİHAZ MODU (SİMÜLASYON KAPALI)
            DeviceCommunication.Instance.SimulationMode = false;

            LogToScreen("🔴 GERÇEK CİHAZ MODU AKTİF");
            LogToScreen("STM32 F411 Nucleo bekleniyor...");
            LogToScreen("Lütfen 'Button10' ile COM9 portunu açın.");

            _dataTimer = new System.Windows.Forms.Timer();
            _dataTimer.Interval = 100; // 100 milisaniye (Saniyede 10 veri)
            _dataTimer.Tick += _dataTimer_Tick; // Her tiklediğinde çalışacak fonksiyon

        }

        private void _dataTimer_Tick(object sender, EventArgs e)
        {
            // Eğer cihaz bağlıysa veri iste
            if (DeviceCommunication.Instance.IsConnected)
            {
                DeviceCommunication.Instance.RequestLoadCellData();
            }
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

        // ✅ TERAPİ BAŞLAT
        private void btnTherapy_Click(object sender, EventArgs e)
        {
            if (!DeviceCommunication.Instance.IsConnected)
            {
                LogToScreen("⚠️ Port açık değil!");
                return;
            }

            // ✅ BACKGROUND THREAD
            Task.Run(() =>
            {
                this.Invoke((MethodInvoker)delegate {
                    LogToScreen("=== TERAPİ BAŞLAT ===");
                });

                bool result = DeviceCommunication.Instance.StartTherapy();

                this.Invoke((MethodInvoker)delegate {
                    if (result)
                        LogToScreen("✓ Terapi başlatıldı!");
                    else
                        LogToScreen("✗ Timeout!");
                });
            });

            // UI güncelle
            Therapy s = new Therapy();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnMonitoring_Click(object sender, EventArgs e)
        {
            // Sadece ekranı açsın, otomatik veri akışını terapi başlatınca yapıyoruz.
            DataMonitoring s = new DataMonitoring();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);

            // İsterseniz anlık tek bir veri görüp bağlantıyı test etmek için şu kalabilir:
            if (DeviceCommunication.Instance.IsConnected)
            {
                LogToScreen("Anlık veri kontrolü için tek istek gönderildi.");
                DeviceCommunication.Instance.RequestLoadCellData();
            }
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
            bool currentMode = DeviceCommunication.Instance.SimulationMode;

            // Modu tersine çevir
            DeviceCommunication.Instance.SimulationMode = !currentMode;

            string modeText = DeviceCommunication.Instance.SimulationMode ? "AKTİF" : "KAPALI";
            string emoji = DeviceCommunication.Instance.SimulationMode ? "🟢" : "🔴";

            LogToScreen($"=== SİMÜLASYON MODU: {modeText} {emoji} ===");

            MessageBox.Show(
                $"Simülasyon Modu: {modeText}\n\n" +
                (DeviceCommunication.Instance.SimulationMode ?
                    "✅ Cihaz yanıtları simüle edilecek (test için)" :
                    "⚠️ Gerçek cihaz yanıtları beklenecek"),
                "Simülasyon Modu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }


        private void button11_Click_1(object sender, EventArgs e)
        {
            LogToScreen("=== PORT KAPATMA ===");

            if (!DeviceCommunication.Instance.IsConnected)
            {
                LogToScreen("Kapatılacak açık port yok!");
                MessageBox.Show("Zaten hiçbir port açık değil.",
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string currentPort = DeviceCommunication.Instance.CurrentPort;

            // Onay al
            DialogResult result = MessageBox.Show(
                $"{currentPort} portunu kapatmak istediğinizden emin misiniz?",
                "Port Kapatma Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LogToScreen($"{currentPort} portu kapatılıyor...");

                bool closeResult = DeviceCommunication.Instance.ClosePort();

                if (closeResult)
                {
                    LogToScreen($"✓ {currentPort} portu başarıyla kapatıldı!");
                    MessageBox.Show($"{currentPort} portu kapatıldı.",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LogToScreen($"✗ Port kapatma hatası!");
                }
            }
            else
            {
                LogToScreen("Port kapatma işlemi iptal edildi.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!DeviceCommunication.Instance.IsConnected)
            {
                LogToScreen("⚠️ Port açık değil! Önce Button10 ile port açın.");
                return;
            }

            // RUN THE TEST IN A BACKGROUND TASK TO AVOID DEADLOCK
            Task.Run(() =>
            {
                // We must use Invoke ONLY when updating UI from this task, 
                // but LogToScreen handles that for us.

                this.Invoke((MethodInvoker)delegate {
                    LogToScreen("=== HIZ OKUMA VE DEĞİŞTİRME TESTİ ===");
                    LogToScreen("1️⃣ Mevcut hız okunuyor...");
                });

                // This call blocks, but now it blocks the Task, not the UI!
                double currentSpeed = DeviceCommunication.Instance.GetCurrentSpeed();

                this.Invoke((MethodInvoker)delegate {
                    if (currentSpeed >= 0)
                        LogToScreen($"   ✓ Mevcut hız: {currentSpeed}");
                    else
                        LogToScreen($"   ✗ Hız okunamadı!");
                });

                // 2. Test Set Speed
                double newSpeed = 75.5;
                this.Invoke((MethodInvoker)delegate { LogToScreen($"2️⃣ Yeni hız ayarlanıyor: {newSpeed}"); });

                bool setResult = DeviceCommunication.Instance.SetSpeedWithConfirmation(newSpeed);

                this.Invoke((MethodInvoker)delegate {
                    if (setResult)
                        LogToScreen($"   ✓ Hız ayarlandı!");
                    else
                        LogToScreen($"   ✗ Hız ayarlanamadı!");

                    LogToScreen("=== TEST TAMAMLANDI ===");
                });
            });
        }

        private void button8_Click(object sender, EventArgs e)//
        {
            // Check connection on UI thread first
            if (!DeviceCommunication.Instance.IsConnected)
            {
                LogToScreen("⚠️ Port açık değil! Önce Button10 ile port açın.");
                return;
            }

            // --- MOVE LOGIC TO BACKGROUND THREAD TO PREVENT DEADLOCK ---
            Task.Run(() =>
            {
                this.Invoke((MethodInvoker)delegate {
                    LogToScreen("=== MOTOR HAREKET TESTİ ===");
                });

                int motorIndex = 1;

                // 1. Get Start Position
                this.Invoke((MethodInvoker)delegate {
                    LogToScreen($"1️⃣ Motor {motorIndex} başlangıç pozisyonu okunuyor...");
                });

                // Blocking call (safe now because we are in Task.Run)
                int startPosition = DeviceCommunication.Instance.GetMotorPosition(motorIndex);

                this.Invoke((MethodInvoker)delegate {
                    if (startPosition >= 0)
                        LogToScreen($"   ✓ Başlangıç pozisyonu: {startPosition}");
                    else
                        LogToScreen($"   ✗ Pozisyon okunamadı!");
                });

                if (startPosition < 0) return; // Stop if read failed

                // 2. Move Motor
                int steps = 100;
                this.Invoke((MethodInvoker)delegate {
                    LogToScreen($"2️⃣ Motor {motorIndex} → {steps} adım ilerletiliyor...");
                });

                bool moveResult = DeviceCommunication.Instance.MoveMotor(motorIndex, steps);

                this.Invoke((MethodInvoker)delegate {
                    if (moveResult)
                        LogToScreen($"   ✓ Motor başarıyla hareket etti!");
                    else
                        LogToScreen($"   ✗ Motor hareket ettirilemedi!");
                });

                if (!moveResult) return;

                // 3. Verify New Position
                Thread.Sleep(200); // Wait for physical movement (simulated)

                this.Invoke((MethodInvoker)delegate {
                    LogToScreen($"3️⃣ Yeni pozisyon okunuyor...");
                });

                int endPosition = DeviceCommunication.Instance.GetMotorPosition(motorIndex);

                this.Invoke((MethodInvoker)delegate {
                    if (endPosition >= 0)
                    {
                        LogToScreen($"   ✓ Yeni pozisyon: {endPosition}");
                        LogToScreen($"   📊 Hareket: {startPosition} → {endPosition} ({endPosition - startPosition} adım)");

                        if (endPosition - startPosition == steps)
                            LogToScreen("   🎉 MOTOR HAREKETİ BAŞARILI!");
                        else
                            LogToScreen($"   ⚠️ Pozisyon uyuşmazlığı! Beklenen: {steps}, Gerçek: {endPosition - startPosition}");
                    }
                    else
                    {
                        LogToScreen($"   ✗ Yeni pozisyon okunamadı!");
                    }
                    LogToScreen("=== TEST TAMAMLANDI ===");
                });
            });
        }
    }
}