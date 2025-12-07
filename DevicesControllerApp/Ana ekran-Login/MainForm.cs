using DevicesControllerApp.Ayarlar;
using DevicesControllerApp.Hasta_kayit;
using DevicesControllerApp.Kullanici;
using DevicesControllerApp.Raporlama;
using DevicesControllerApp.Servis;
using DevicesControllerApp.Terapi;
using DevicesControllerApp.Veri_izleme;
using RehabilitationSystem.Mobile;
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
        private MobileCommandServer _mobileServer;
        private ToolStripStatusLabel _connectionStatusLabel;
        private ToolStripStatusLabel _therapyStatusLabel;

        public MainForm()
        {
            InitializeComponent();
            InitializeMobileBridge();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Settings s= new Settings();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

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

        private void btnMonitoring_Click(object sender, EventArgs e)
        {
            DataMonitoring s = new DataMonitoring();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            UserRegistration s = new UserRegistration();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        private void btnTherapy_Click(object sender, EventArgs e)
        {
            Therapy s = new Therapy();
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
        }

        /// <summary>
        /// Mobil komut sunucusunu ve durum göstergelerini kurar.
        /// </summary>
        private void InitializeMobileBridge()
        {
            _connectionStatusLabel = new ToolStripStatusLabel("Mobil bağlantı: kapalı");
            _therapyStatusLabel = new ToolStripStatusLabel("Terapi durumu: hazır");
            statusStrip1.Items.Add(_connectionStatusLabel);
            statusStrip1.Items.Add(new ToolStripStatusLabel { Spring = true });
            statusStrip1.Items.Add(_therapyStatusLabel);

            try
            {
                _mobileServer = new MobileCommandServer();
                _mobileServer.ClientConnected += MobileServer_ClientConnected;
                _mobileServer.ClientDisconnected += MobileServer_ClientDisconnected;
                _mobileServer.TherapyStateChanged += MobileServer_TherapyStateChanged;
                _mobileServer.CommandProcessed += MobileServer_CommandProcessed;
                _mobileServer.Start();
                UpdateConnectionStatus("Mobil sunucu: 127.0.0.1:9000");
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus("Mobil sunucu başlatılamadı");
                _therapyStatusLabel.Text = $"Terapi durumu: hata ({ex.Message})";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (_mobileServer != null)
            {
                _mobileServer.Dispose();
            }
        }

        private void MobileServer_CommandProcessed(object sender, string e)
        {
            UpdateTherapyStatus($"Son komut: {e}");
        }

        private void MobileServer_TherapyStateChanged(object sender, TherapySessionState e)
        {
            var statusText = BuildTherapyStatusText(e);
            UpdateTherapyStatus(statusText);
        }

        private void MobileServer_ClientDisconnected(object sender, string e)
        {
            UpdateConnectionStatus("Mobil bağlantı: kapalı");
        }

        private void MobileServer_ClientConnected(object sender, string e)
        {
            UpdateConnectionStatus($"Mobil bağlantı: {e}");
        }

        private void UpdateConnectionStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateConnectionStatus(text)));
                return;
            }

            _connectionStatusLabel.Text = text;
        }

        private void UpdateTherapyStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateTherapyStatus(text)));
                return;
            }

            _therapyStatusLabel.Text = text;
        }

        private string BuildTherapyStatusText(TherapySessionState state)
        {
            if (state == null)
            {
                return "Terapi durumu: bilinmiyor";
            }

            if (state.IsEmergency)
            {
                return "Terapi durumu: ACİL STOP";
            }

            if (state.IsRunning && state.IsPaused)
            {
                return "Terapi durumu: beklemede";
            }

            if (state.IsRunning)
            {
                return "Terapi durumu: devam ediyor";
            }

            return "Terapi durumu: hazır";
        }
    }
}
