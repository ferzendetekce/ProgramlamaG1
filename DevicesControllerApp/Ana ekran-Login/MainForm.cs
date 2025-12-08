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
        private Button _btnDisconnect;
        private Label _lblConnection;
        private Label _lblTherapy;
        private Label _lblApi;
        private Label _lblWeight;
        private Label _lblShoe;
        private Label _lblSupport;
        private Label _lblLastCommand;
        private Panel _winchPanel;
        private Panel _mobilePanel;
        private Label _lblElapsed;
        private Panel _chartPanel;
        private readonly Queue<int> _progressHistory = new Queue<int>();
        private readonly Random _rand = new Random();
        private double _winchPosition = 50;
        private int _lastWinchSerial = -1;
        private Timer _therapyTimer;
        private TherapySessionState _lastTherapyState;

        private NumericUpDown _numTargetMinutes;
        private NumericUpDown _numWeightInput;
        private NumericUpDown _numShoeInput;
        private NumericUpDown _numSupportInput;

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

        private void MobileServer_CommandProcessed(object sender, string e)
        {
            UpdateTherapyStatus(BuildTherapyStatusText(_lastTherapyState));
            if (!string.IsNullOrEmpty(e) && e.ToLowerInvariant().Contains("disconnect"))
            {
                UpdateConnectionStatus("Mobil baglanti: kapali", false);
            }
        }

        private void MobileServer_TherapyStateChanged(object sender, TherapySessionState e)
        {
            _lastTherapyState = e;
            UpdateTherapyStatus(BuildTherapyStatusText(e));
            UpdateTherapyProgress(e);
        }

        private void MobileServer_ClientDisconnected(object sender, string e) =>
            UpdateConnectionStatus("Mobil baglanti: kapali", false);

        private void MobileServer_ClientConnected(object sender, string e) =>
            UpdateConnectionStatus($"Mobil baglanti: {e}", true);

        private void UpdateConnectionStatus(string text, bool connected = false)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateConnectionStatus(text, connected)));
                return;
            }
            _lblConnection.Text = text;
            _lblConnection.ForeColor = connected ? Color.ForestGreen : Color.Maroon;
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
            return "Terapi durumu: hazir";
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
                Text = "API Baslat",
                Width = 110,
                Height = 35,
                BackColor = Color.LightGreen
            };
            _btnStartApi.Click += (s, e) =>
            {
                StartMobileStack();
            };

            _btnStopApi = new Button
            {
                Text = "API Durdur",
                Width = 110,
                Height = 35,
                BackColor = Color.LightCoral
            };
            _btnStopApi.Click += (s, e) => StopMobileStack();

            _btnDisconnect = new Button
            {
                Text = "Baglantiyi Kes",
                Width = 120,
                Height = 35,
                BackColor = Color.LightGray
            };
            _btnDisconnect.Click += (s, e) => StopMobileStack();

            _lblConnection = new Label
            {
                Text = "Mobil baglanti: kapali",
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };

            _lblTherapy = new Label
            {
                Text = "Terapi durumu: hazir",
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };

            _lblApi = new Label
            {
                Text = "EngineAPI: kapali",
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };

            _mobileToolbar.Controls.Add(_btnStartApi);
            _mobileToolbar.Controls.Add(_btnStopApi);
            _mobileToolbar.Controls.Add(_btnDisconnect);
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
                    UpdateConnectionStatus("Mobil baglanti: dinleniyor", false);
                    _lblTherapy.Text = "Terapi durumu: hazir";
                    _lblApi.Text = $"EngineAPI: {_mobileService.ApiHost.Port} (calisiyor)";
                }
                else
                {
                    _lblApi.Text = $"EngineAPI: hata ({error})";
                    MessageBox.Show(error, "EngineAPI baslatilamadi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _lblApi.Text = $"EngineAPI: hata ({ex.Message})";
                MessageBox.Show(ex.Message, "EngineAPI baslatilamadi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopMobileStack()
        {
            _mobileService.StopAll();
            _lastTherapyState = null;
            UpdateConnectionStatus("Mobil baglanti: kapali", false);
            _lblApi.Text = "EngineAPI: durduruldu";
            _lblTherapy.Text = "Terapi durumu: hazir";
            _lblElapsed.Text = "Sure: 00:00 / 00:00";
            _lblWeight.Text = "Agirlik Azaltma: -";
            _lblShoe.Text = "Ayak Numarasi: -";
            _lblSupport.Text = "Destek Bari: -";
            _lblLastCommand.Text = "Son Komut: -";
            _progressHistory.Clear();
            _chartPanel.Invalidate();
        }

        private Panel BuildTherapySetupPanel()
        {
            var setupPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(8),
                BorderStyle = BorderStyle.FixedSingle
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2
            };
            for (int i = 0; i < 4; i++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            _numTargetMinutes = new NumericUpDown
            {
                Minimum = 5,
                Maximum = 180,
                Value = 30,
                DecimalPlaces = 0,
                Dock = DockStyle.Fill
            };
            _numWeightInput = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 150,
                DecimalPlaces = 0,
                Increment = 1M,
                Value = 20,
                Dock = DockStyle.Fill
            };
            _numShoeInput = new NumericUpDown
            {
                Minimum = 20,
                Maximum = 50,
                Value = 42,
                Dock = DockStyle.Fill
            };
            _numSupportInput = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 200,
                DecimalPlaces = 2,
                Increment = 0.05M,
                Value = 0.40M,
                Dock = DockStyle.Fill
            };

            grid.Controls.Add(new Label { Text = "Sure (dk)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 0);
            grid.Controls.Add(new Label { Text = "Agirlik Azaltma (kg)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 1, 0);
            grid.Controls.Add(new Label { Text = "Ayak No", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 2, 0);
            grid.Controls.Add(new Label { Text = "Destek Bari (m)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 3, 0);

            grid.Controls.Add(_numTargetMinutes, 0, 1);
            grid.Controls.Add(_numWeightInput, 1, 1);
            grid.Controls.Add(_numShoeInput, 2, 1);
            grid.Controls.Add(_numSupportInput, 3, 1);

            _numTargetMinutes.ValueChanged += (s, e) => UpdatePresetLabels();
            _numWeightInput.ValueChanged += (s, e) => UpdatePresetLabels();
            _numShoeInput.ValueChanged += (s, e) => UpdatePresetLabels();
            _numSupportInput.ValueChanged += (s, e) => UpdatePresetLabels();

            setupPanel.Controls.Add(grid);
            return setupPanel;
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

            var setupPanel = BuildTherapySetupPanel();

            var therapyPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.White,
                Padding = new Padding(12),
                BorderStyle = BorderStyle.FixedSingle
            };

            var therapyTitle = new Label
            {
                Text = "Terapi Sureci",
                Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 22
            };

            _lblElapsed = new Label
            {
                Text = "Sure: 00:00 / 00:00",
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

            _lblWeight = CreateInfoLabel("Agirlik Azaltma: -");
            _lblShoe = CreateInfoLabel("Ayak Numarasi: -");
            _lblSupport = CreateInfoLabel("Destek Bari: -");
            _lblLastCommand = CreateInfoLabel("Son Komut: -");

            detailsPanel.Controls.Add(_lblWeight, 0, 0);
            detailsPanel.Controls.Add(_lblShoe, 1, 0);
            detailsPanel.Controls.Add(_lblSupport, 0, 1);
            detailsPanel.Controls.Add(_lblLastCommand, 1, 1);

            therapyPanel.Controls.Add(_lblElapsed);
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
                Text = "Son Terapiler Ilerleme Grafigi",
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };
            _chartPanel.Controls.Add(chartTitle);

            card.Controls.Add(_chartPanel);
            card.Controls.Add(BuildWinchPanel());
            card.Controls.Add(therapyPanel);
            card.Controls.Add(setupPanel);
            card.Controls.Add(statusPanel);

            _mobilePanel.Controls.Add(card);
            _mobilePanel.Controls.Add(header);
        }

        private void UpdateTherapyProgress(TherapySessionState state)
        {
            if (_lblElapsed == null)
            {
                return;
            }

            if (state == null || !state.StartedAt.HasValue || !state.LastUpdate.HasValue || state.TargetDurationMinutes <= 0)
            {
                _lblElapsed.Text = "Sure: 00:00 / 00:00";
                _lblLastCommand.Text = "Son Komut: -";
                return;
            }

            var elapsed = state.LastUpdate.Value - state.StartedAt.Value;
            if (elapsed.TotalSeconds < 0)
            {
                elapsed = TimeSpan.Zero;
            }

            var target = TimeSpan.FromMinutes(state.TargetDurationMinutes);

            var elapsedText = $"{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}";
            var targetText = $"{state.TargetDurationMinutes:00}:00";
            _lblElapsed.Text = $"Sure: {elapsedText} / {targetText}";

            _lblWeight.Text = $"Agirlik Azaltma: {Math.Round(state.WeightSupport, 1)} kg";
            _lblShoe.Text = $"Ayak Numarasi: {state.ShoeSize}";
            _lblSupport.Text = $"Destek Bari: {state.SupportBarHeight:0.00} m";
            _lblLastCommand.Text = $"Son Komut: {state.LastCommand}";

            var percent = _rand.Next(0, 101);

            UpdateWinchDemo(state);

            _progressHistory.Enqueue(percent);
            while (_progressHistory.Count > 20)
            {
                _progressHistory.Dequeue();
            }
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
            if (_lastTherapyState != null &&
                _lastTherapyState.StartedAt.HasValue &&
                _lastTherapyState.TargetDurationMinutes > 0 &&
                _lastTherapyState.IsRunning &&
                !_lastTherapyState.IsPaused &&
                !_lastTherapyState.IsEmergency)
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

        private void UpdatePresetLabels()
        {
            _lblWeight.Text = $"Agirlik Azaltma: {_numWeightInput.Value:0.0} kg";
            _lblShoe.Text = $"Ayak Numarasi: {(int)_numShoeInput.Value}";
            _lblSupport.Text = $"Destek Bari: {_numSupportInput.Value:0.00} m";
            _lblElapsed.Text = $"Sure: 00:00 / {_numTargetMinutes.Value:00}:00";
            if (_lastTherapyState != null)
            {
                _lastTherapyState.TargetDurationMinutes = (int)_numTargetMinutes.Value;
                _lastTherapyState.WeightSupport = (double)_numWeightInput.Value;
                _lastTherapyState.ShoeSize = (int)_numShoeInput.Value;
                _lastTherapyState.SupportBarHeight = (double)_numSupportInput.Value;
            }
        }

        private void UpdateWinchDemo(TherapySessionState state)
        {
            if (state == null || string.IsNullOrWhiteSpace(state.LastCommand))
            {
                return;
            }

            if (state.CommandSerial <= _lastWinchSerial)
            {
                return;
            }

            var cmd = state.LastCommand.ToLowerInvariant();
            if (cmd.Contains("yukari"))
            {
                _winchPosition = Math.Min(100, _winchPosition + 5);
            }
            else if (cmd.Contains("asagi") || cmd.Contains("aşa") || cmd.Contains("aYa"))
            {
                _winchPosition = Math.Max(0, _winchPosition - 5);
            }

            _lastWinchSerial = state.CommandSerial;
            _winchPanel?.Invalidate();
        }

        private Panel BuildWinchPanel()
        {
            _winchPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 140,
                BackColor = Color.White,
                Padding = new Padding(6),
                BorderStyle = BorderStyle.FixedSingle
            };
            _winchPanel.Paint += WinchPanel_Paint;
            return _winchPanel;
        }

        private void WinchPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            var bounds = _winchPanel.ClientRectangle;
            bounds.Inflate(-8, -8);
            var rail = new Rectangle(bounds.Left + bounds.Width / 2 - 8, bounds.Top, 16, bounds.Height);
            using (var railPen = new Pen(Color.LightGray, 3))
            {
                g.DrawLine(railPen, rail.Left + rail.Width / 2, rail.Top, rail.Left + rail.Width / 2, rail.Bottom);
            }

            var figureHeight = 30;
            var y = bounds.Bottom - (int)(_winchPosition / 100f * bounds.Height) - figureHeight;
            var figRect = new Rectangle(rail.Left - 12, y, rail.Width + 24, figureHeight);
            using (var bodyBrush = new SolidBrush(Color.SteelBlue))
            {
                g.FillRectangle(bodyBrush, figRect);
            }
            using (var textBrush = new SolidBrush(Color.Black))
            using (var font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold))
            {
                g.DrawString("Vinc", font, textBrush, figRect.Left, figRect.Top - 14);
            }
        }
    }
}
