using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DevicesControllerApp.Veri_izleme
{
    public partial class DataMonitoringControl : UserControl
    {
        private const int StepPoints = 100;
        private Timer timer;

        private double[] rightHeel = new double[StepPoints];
        private double[] leftHeel = new double[StepPoints];
        private double[] rightToe = new double[StepPoints];
        private double[] leftToe = new double[StepPoints];

        private int timeIndex = 0;
        private int pointIndex = 0;
        private int stepCount = 0;

        private string currentLanguage = "tr";
        private bool isFrozen = false;

        public DataMonitoringControl()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 50; // 20 FPS
            timer.Tick += Timer_Tick;

            this.Disposed += DataMonitoringControl_Disposed;
        }

        private void DataMonitoringControl_Load(object sender, EventArgs e)
        {
            cmbLanguage.Items.Clear();
            cmbLanguage.Items.Add("Türkçe");
            cmbLanguage.Items.Add("English");
            cmbLanguage.SelectedIndex = 0;

            ApplyLanguage();
            InitCharts();
        }

        private void StyleChart(Chart ch)
        {
            ch.BackColor = Color.White;
            ch.ChartAreas[0].BackColor = Color.WhiteSmoke;

            ch.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            ch.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

            ch.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
            ch.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;

            ch.AntiAliasing = AntiAliasingStyles.All;
        }

        private void InitCharts()
        {
            SetupStepChart(chart1, Color.Blue);
            SetupStepChart(chart2, Color.Orange);
            SetupStepChart(chart3, Color.Green);
            SetupStepChart(chart4, Color.Purple);
            SetupTimeChart(chart5, Color.DarkCyan);

            StyleChart(chart1);
            StyleChart(chart2);
            StyleChart(chart3);
            StyleChart(chart4);
            StyleChart(chart5);

            AttachZoom(chart1);
            AttachZoom(chart2);
            AttachZoom(chart3);
            AttachZoom(chart4);
            AttachZoom(chart5);

            RedrawStepCharts();
        }

        private void SetupStepChart(Chart chart, Color lineColor)
        {
            chart.Series.Clear();

            Series s = new Series("Live");
            s.ChartType = SeriesChartType.Line;
            s.BorderWidth = 3;
            s.Color = lineColor;

            chart.Series.Add(s);

            var area = chart.ChartAreas[0];
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = StepPoints - 1;
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 100;
        }

        private void SetupTimeChart(Chart chart, Color lineColor)
        {
            chart.Series.Clear();

            Series s = new Series("Live");
            s.ChartType = SeriesChartType.Line;
            s.BorderWidth = 3;
            s.Color = lineColor;

            chart.Series.Add(s);

            var area = chart.ChartAreas[0];
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 200;
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 100;
        }

        // ------------------ DATA UPDATE (20 FPS) -----------------
        private void Timer_Tick(object sender, EventArgs e)
        {
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            double v4 = 0;
            double vb = 0;

            rightHeel[pointIndex] = v1;
            leftHeel[pointIndex] = v2;
            rightToe[pointIndex] = v3;
            leftToe[pointIndex] = v4;

            if (!isFrozen)
            {
                RedrawStepCharts();
                AppendBalancePoint(vb);
            }

            pointIndex++;
            if (pointIndex >= StepPoints)
            {
                pointIndex = 0;
                stepCount++;

                lblStepCount.Text = currentLanguage == "tr"
                    ? $"Adım Sayısı: {stepCount}"
                    : $"Step Count: {stepCount}";
            }
        }

        private void RedrawStepCharts()
        {
            Draw(chart1, rightHeel);
            Draw(chart2, leftHeel);
            Draw(chart3, rightToe);
            Draw(chart4, leftToe);
        }

        private void Draw(Chart chart, double[] buffer)
        {
            var s = chart.Series[0];
            s.Points.Clear();

            for (int i = 0; i < StepPoints; i++)
                s.Points.AddXY(i, buffer[i]);
        }

        private void AppendBalancePoint(double value)
        {
            var s = chart5.Series[0];

            s.Points.AddXY(timeIndex, value);

            if (s.Points.Count > 200)
                s.Points.RemoveAt(0);

            chart5.ChartAreas[0].AxisX.Minimum = Math.Max(0, timeIndex - 200);
            chart5.ChartAreas[0].AxisX.Maximum = timeIndex;

            timeIndex++;
        }

        private void DataMonitoringControl_Disposed(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Dispose();
        }

        // ------------------ BUTTONS ------------------
        private void btnStart_Click(object sender, EventArgs e) => timer.Start();
        private void btnStop_Click(object sender, EventArgs e) => timer.Stop();

        private void btnFreeze_Click(object sender, EventArgs e)
        {
            if (!isFrozen)
            {
                CreateFrozen(chart1);
                CreateFrozen(chart2);
                CreateFrozen(chart3);
                CreateFrozen(chart4);
                CreateFrozen(chart5);

                isFrozen = true;
                btnFreeze.Text = currentLanguage == "tr" ? "Çöz" : "Unfreeze";
            }
            else
            {
                RemoveFrozen(chart1);
                RemoveFrozen(chart2);
                RemoveFrozen(chart3);
                RemoveFrozen(chart4);
                RemoveFrozen(chart5);

                isFrozen = false;
                btnFreeze.Text = currentLanguage == "tr" ? "Dondur" : "Freeze";

                RedrawStepCharts();
            }
        }

        private void CreateFrozen(Chart chart)
        {
            if (chart.Series.IndexOf("Frozen") != -1)
                return;

            Series frozen = new Series("Frozen");
            frozen.ChartType = SeriesChartType.Line;
            frozen.BorderWidth = 2;
            frozen.Color = Color.Red;

            foreach (var p in chart.Series[0].Points)
                frozen.Points.AddXY(p.XValue, p.YValues[0]);

            chart.Series.Add(frozen);
        }

        private void RemoveFrozen(Chart chart)
        {
            if (chart.Series.IndexOf("Frozen") != -1)
                chart.Series.Remove(chart.Series["Frozen"]);
        }

        // ------------------ EXPORT ------------------
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "PNG|*.png";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(this.Width, this.Height);
                    this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Width, this.Height));
                    bmp.Save(dlg.FileName);

                    MessageBox.Show(currentLanguage == "tr"
                        ? "Kayıt başarılı!"
                        : "Saved successfully!");
                }
            }
        }

        // ------------------ COMPARE ------------------
        private void btnCompare_Click(object sender, EventArgs e)
        {
            double a = rightHeel[pointIndex];
            double b = leftHeel[pointIndex];

            if (currentLanguage == "tr")
                MessageBox.Show(a > b ? "Sağ Topuk daha yüksek" : "Sol Topuk daha yüksek");
            else
                MessageBox.Show(a > b ? "Right heel is higher" : "Left heel is higher");
        }

        // ------------------ LANGUAGE ------------------
        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentLanguage = cmbLanguage.SelectedIndex == 0 ? "tr" : "en";
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            if (currentLanguage == "tr")
            {
                label1.Text = "REHABİLİTASYON VERİSİ İZLEME";
                btnStart.Text = "BAŞLA";
                btnStop.Text = "Durdur";
                btnFreeze.Text = isFrozen ? "Çöz" : "Dondur";
                btnExport.Text = "Dışa Aktar";
                btnCompare.Text = "Karşılaştırma";
                lblStepCount.Text = $"Adım Sayısı: {stepCount}";

                chart1.Titles.Clear(); chart1.Titles.Add("Sağ Topuk Sensörü");
                chart2.Titles.Clear(); chart2.Titles.Add("Sol Topuk Sensörü");
                chart3.Titles.Clear(); chart3.Titles.Add("Sağ Parmak Sensörü");
                chart4.Titles.Clear(); chart4.Titles.Add("Sol Parmak Sensörü");
                chart5.Titles.Clear(); chart5.Titles.Add("Denge Sensörü");
            }
            else
            {
                label1.Text = "REHABILITATION DATA MONITORING";
                btnStart.Text = "Start";
                btnStop.Text = "Stop";
                btnFreeze.Text = isFrozen ? "Unfreeze" : "Freeze";
                btnExport.Text = "Export";
                btnCompare.Text = "Compare";
                lblStepCount.Text = $"Step Count: {stepCount}";

                chart1.Titles.Clear(); chart1.Titles.Add("Right Heel Sensor");
                chart2.Titles.Clear(); chart2.Titles.Add("Left Heel Sensor");
                chart3.Titles.Clear(); chart3.Titles.Add("Right Toe Sensor");
                chart4.Titles.Clear(); chart4.Titles.Add("Left Toe Sensor");
                chart5.Titles.Clear(); chart5.Titles.Add("Balance Sensor");
            }
        }

        // ------------------ ZOOM + TOOLTIP ------------------
        private void AttachZoom(Chart chart)
        {
            chart.MouseWheel += Chart_MouseWheel;
            chart.MouseMove += Chart_MouseMove;
        }

        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = sender as Chart;
            var area = chart.ChartAreas[0];

            try
            {
                double xMin = area.AxisX.ScaleView.ViewMinimum;
                double xMax = area.AxisX.ScaleView.ViewMaximum;
                double pos = area.AxisX.PixelPositionToValue(e.Location.X);

                if (e.Delta < 0)
                    area.AxisX.ScaleView.ZoomReset();
                else
                {
                    double newSize = (xMax - xMin) * 0.7;
                    area.AxisX.ScaleView.Zoom(pos - newSize / 2, pos + newSize / 2);
                }
            }
            catch { }
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            var chart = sender as Chart;
            var hit = chart.HitTest(e.X, e.Y);

            if (hit.ChartElementType == ChartElementType.DataPoint)
            {
                var p = chart.Series[hit.Series.Name].Points[hit.PointIndex];
                chart.Series[0].ToolTip = $"X={p.XValue}, Y={p.YValues[0]}";
            }
            else
                chart.Series[0].ToolTip = "";
        }
    }
}
