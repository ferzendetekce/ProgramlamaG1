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
using System.Drawing;
using System.Windows.Forms;

namespace DevicesControllerApp
{
    public partial class MainForm : Form
    {
        private MobileService _mobileService;
        private FlowLayoutPanel _mobileToolbar;
        private Button _btnStartApi;
        private Button _btnStopApi;
        private Button _btnDebug;
        private Label _lblConnection;
        private Label _lblTherapy;
        private Label _lblApi;
        private Label _lblWeight;
        private Label _lblShoe;
        private Label _lblSupport;
        private Label _lblStatus;
        private Panel _mobilePanel;
        private ProgressBar _progressTherapy;
        private Label _lblElapsed;
        private Panel _chartPanel;
        private readonly Queue<int> _progressHistory = new Queue<int>();
        private Timer _therapyTimer;
        private TherapySessionState _lastTherapyState;

        public MainForm()
        {
            InitializeComponent();
            InitializeMobileBridge();
            button8.Click += BtnMobile_Click;
        }

        private void btnSettings_Click(object sender, EventArgs e) => ShowControl(new Settings());
        private void btnPatient_Click(object sender, EventArgs e) => ShowControl(new PatientRegistration());
        private void btnReports_Click(object sender, EventArgs e) => ShowControl(new Reports());
        private void btnService_Click(object sender, EventArgs e) => ShowControl(new Service());
        private void btnMonitoring_Click(object sender, EventArgs e) => ShowControl(new DataMonitoring());
        private void btnUsers_Click(object sender, EventArgs e) => ShowControl(new UserRegistration());
        private void btnTherapy_Click(object sender, EventArgs e) => ShowControl(new Therapy());

        private void ShowControl(UserControl control)
        {
            splitContainer2.Panel2.Controls.Clear();
            control.Dock = DockStyle.Fill;
            splitContainer2.Panel2.Controls.Add(control);
        }

        /// <summary>
        /// Mobil komut sunucusunu ve durum göstergelerini kurar.
        /// </summary>
        private void InitializeMobileBridge()
        {
            _mobileService = new MobileService();
            _mobileService.CommandServer.ClientConnected += MobileServer_ClientConnected;
            _mobileService.CommandServer.ClientDisconnected += MobileServer_ClientDisconnected;
            _mobileService.CommandServer.TherapyStateChanged += MobileServer_TherapyStateChanged;
            _mobileService.CommandServer.CommandProcessed += MobileServer_CommandProcessed;

            _therapyTimer = new Timer { Interval = 1000 };
            _therapyTimer.Tick += TherapyTimer_Tick;
            _therapyTimer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _mobileService?.Dispose();
            _therapyTimer?.Stop();
            _therapyTimer?.Dispose();
        }

        private void MobileServer_CommandProcessed(object sender, string e) =>
            UpdateTherapyStatus($"Son komut: {e}");

        private void MobileServer_TherapyStateChanged(object sender, TherapySessionState e)
        {
            _lastTherapyState = e;
            UpdateTherapyStatus(BuildTherapyStatusText(e));
            UpdateTherapyProgress(e);
        }

        private void MobileServer_ClientDisconnected(object sender, string e) =>
            UpdateConnectionStatus("Mobil bağlantı: kapalı");

        private void MobileServer_ClientConnected(object sender, string e) =>
            UpdateConnectionStatus($"Mobil bağlantı: {e}");

        private void UpdateConnectionStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateConnectionStatus(text)));
                return;
            }

            _lblConnection.Text = text;
        }

        private void UpdateTherapyStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateTherapyStatus(text)));
                return;
            }

            _lblTherapy.Text = text;
        }

        private string BuildTherapyStatusText(TherapySessionState state)
        {
            if (state == null)
                return "Terapi durumu: bilinmiyor";
            if (state.IsEmergency)
                return "Terapi durumu: ACIL STOP";
            if (state.IsRunning && state.IsPaused)
                return "Terapi durumu: beklemede";
            if (state.IsRunning)
                return "Terapi durumu: devam ediyor";
            return "Terapi durumu: hazır";
        }

        private void BtnMobile_Click(object sender, EventArgs e) => ShowMobilePanel();

        private void ShowMobilePanel()
        {
            if (_mobilePanel == null)
            {
                BuildMobilePanel();
            }

            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(_mobilePanel);
        }

        private void BuildMobileToolbar()
        {
            _mobileToolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(10),
                AutoScroll = true
            };

            _btnStartApi = new Button
            {
                Text = "API Başlat",
                Width = 110,
                Height = 35,
                BackColor = Color.LightGreen
            };
            _btnStartApi.Click += (s, e) => StartMobileStack();

            _btnStopApi = new Button
            {
                Text = "API Durdur",
                Width = 110,
                Height = 35,
                BackColor = Color.LightCoral
            };
            _btnStopApi.Click += (s, e) => StopMobileStack();

            _btnDebug = new Button
            {
                Text = "Debug",
                Width = 90,
                Height = 35,
                BackColor = Color.LightYellow
            };
            _btnDebug.Click += (s, e) => ShowDebugInfo();

            _lblConnection = new Label
            {
                Text = "Mobil bağlantı: kapalı",
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };

            _lblTherapy = new Label
            {
                Text = "Terapi durumu: hazır",
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };

            _lblApi = new Label
            {
                Text = "EngineAPI: kapalı",
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };

            _mobileToolbar.Controls.Add(_btnStartApi);
            _mobileToolbar.Controls.Add(_btnStopApi);
            _mobileToolbar.Controls.Add(_btnDebug);
            _mobileToolbar.Controls.Add(_lblConnection);
            _mobileToolbar.Controls.Add(_lblTherapy);
            _mobileToolbar.Controls.Add(_lblApi);
        }

        private void StartMobileStack()
        {
            try
            {
                if (_mobileService.StartAll(out var error))
                {
                    UpdateConnectionStatus("Mobil bağlantı: dinleniyor");
                    _lblTherapy.Text = "Terapi durumu: hazır";
                    _lblApi.Text = $"EngineAPI: {_mobileService.ApiHost.Port} (çalışıyor)";
                }
                else
                {
                    _lblApi.Text = $"EngineAPI: hata ({error})";
                    MessageBox.Show(error, "EngineAPI Başlatılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _lblApi.Text = $"EngineAPI: hata ({ex.Message})";
                MessageBox.Show(ex.Message, "EngineAPI Başlatılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopMobileStack()
        {
            _mobileService.StopAll();
            UpdateConnectionStatus("Mobil bağlantı: kapalı");
            _lblApi.Text = "EngineAPI: durduruldu";
            _lblTherapy.Text = "Terapi durumu: hazır";
            _progressTherapy.Value = 0;
            _lblElapsed.Text = "Süre: 00:00 / 00:00";
            _lblWeight.Text = "Ağırlık Azaltma: -";
            _lblShoe.Text = "Ayak Numarası: -";
            _lblSupport.Text = "Destek Barı: -";
            _lblStatus.Text = "Durum: hazır";
        }

        private void ShowDebugInfo()
        {
            var info = $"API Portu: {_mobileService.ApiHost.Port}\nKomut Sunucusu: {_mobileService.CommandServer?.IsRunning}\nDiscovery: {_mobileService.DiscoveryServer?.IsRunning}\nSon Hata: {_mobileService.ApiHost.LastError}";
            MessageBox.Show(info, "Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BuildMobilePanel()
        {
            _mobilePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            BuildMobileToolbar();

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.LightSteelBlue
            };
            var title = new Label
            {
                Text = "Mobil Kontrol Merkezi",
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                Dock = DockStyle.Left,
                Padding = new Padding(15, 20, 0, 0)
            };
            header.Controls.Add(title);

            var card = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = Color.WhiteSmoke
            };

            var statusPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
            statusPanel.Controls.Add(_mobileToolbar);

            var therapyPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                BackColor = Color.White,
                Padding = new Padding(12),
                BorderStyle = BorderStyle.FixedSingle
            };

            var therapyTitle = new Label
            {
                Text = "Terapi Süreci",
                Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 22
            };

            _progressTherapy = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 24,
                Minimum = 0,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };

            _lblElapsed = new Label
            {
                Text = "Süre: 00:00 / 00:00",
                Dock = DockStyle.Top,
                Height = 20,
                Padding = new Padding(0, 6, 0, 0)
            };

            var detailsPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                Dock = DockStyle.Top,
                Height = 70
            };
            detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34));

            _lblWeight = CreateInfoLabel("Ağırlık Azaltma: -");
            _lblShoe = CreateInfoLabel("Ayak Numarası: -");
            _lblSupport = CreateInfoLabel("Destek Barı: -");
            _lblStatus = CreateInfoLabel("Durum: hazır");

            detailsPanel.Controls.Add(_lblWeight, 0, 0);
            detailsPanel.Controls.Add(_lblShoe, 1, 0);
            detailsPanel.Controls.Add(_lblSupport, 0, 1);
            detailsPanel.Controls.Add(_lblStatus, 1, 1);

            therapyPanel.Controls.Add(_lblElapsed);
            therapyPanel.Controls.Add(_progressTherapy);
            therapyPanel.Controls.Add(therapyTitle);
            therapyPanel.Controls.Add(detailsPanel);

            _chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
            _chartPanel.Paint += ChartPanel_Paint;

            var chartTitle = new Label
            {
                Text = "Son Terapiler İlerleme Grafiği",
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };
            _chartPanel.Controls.Add(chartTitle);

            card.Controls.Add(_chartPanel);
            card.Controls.Add(therapyPanel);
            card.Controls.Add(statusPanel);

            _mobilePanel.Controls.Add(card);
            _mobilePanel.Controls.Add(header);
        }

        private void UpdateTherapyProgress(TherapySessionState state)
        {
            if (_progressTherapy == null || _lblElapsed == null)
            {
                return;
            }

            if (state == null || !state.StartedAt.HasValue || !state.LastUpdate.HasValue || state.TargetDurationMinutes <= 0)
            {
                _progressTherapy.Value = 0;
                _lblElapsed.Text = "Süre: 00:00 / 00:00";
                return;
            }

            var elapsed = state.LastUpdate.Value - state.StartedAt.Value;
            if (elapsed.TotalSeconds < 0)
            {
                elapsed = TimeSpan.Zero;
            }

            var target = TimeSpan.FromMinutes(state.TargetDurationMinutes);
            var percent = target.TotalSeconds > 0
                ? Math.Min(100, (int)((elapsed.TotalSeconds / target.TotalSeconds) * 100))
                : 0;

            _progressTherapy.Value = Math.Max(0, percent);

            var elapsedText = $"{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}";
            var targetText = $"{state.TargetDurationMinutes:00}:00";
            _lblElapsed.Text = $"Süre: {elapsedText} / {targetText}";

            if (_lblWeight != null)
            {
                _lblWeight.Text = $"Ağırlık Azaltma: {Math.Round(state.WeightSupport, 1)} kg";
            }

            if (_lblShoe != null)
            {
                _lblShoe.Text = $"Ayak Numarası: {state.ShoeSize}";
            }

            if (_lblSupport != null)
            {
                _lblSupport.Text = $"Destek Barı: {state.SupportBarHeight:0.00} m";
            }

            if (_lblStatus != null)
            {
                _lblStatus.Text = $"Durum: {BuildTherapyStatusText(state)}";
            }

            if (_progressHistory.Count > 30)
            {
                _progressHistory.Dequeue();
            }
            _progressHistory.Enqueue(percent);
            _chartPanel.Invalidate();

            _lastTherapyState = state;
        }

        private void ChartPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            var bounds = _chartPanel.ClientRectangle;
            bounds.Inflate(-10, -10);
            if (bounds.Width <= 0 || bounds.Height <= 0 || _progressHistory.Count == 0)
            {
                return;
            }

            var data = _progressHistory.ToArray();
            var maxPoints = Math.Max(1, data.Length - 1);
            var stepX = bounds.Width / (float)maxPoints;
            var scaleY = bounds.Height / 100f;

            using (var pen = new Pen(Color.SteelBlue, 2))
            {
                for (int i = 0; i < data.Length - 1; i++)
                {
                    var x1 = bounds.Left + i * stepX;
                    var y1 = bounds.Bottom - data[i] * scaleY;
                    var x2 = bounds.Left + (i + 1) * stepX;
                    var y2 = bounds.Bottom - data[i + 1] * scaleY;
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }

        private void TherapyTimer_Tick(object sender, EventArgs e)
        {
            if (_lastTherapyState != null && _lastTherapyState.StartedAt.HasValue && _lastTherapyState.TargetDurationMinutes > 0)
            {
                _lastTherapyState.LastUpdate = DateTime.UtcNow;
                UpdateTherapyProgress(_lastTherapyState);
            }
        }

        private Label CreateInfoLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular),
                Padding = new Padding(0, 4, 0, 0)
            };
        }
    }
}
