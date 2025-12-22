using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using RehabilitationSystem.Communication;
using DevicesControllerApp.Database;

namespace DevicesControllerApp.Servis
{
    public partial class Service : UserControl
    {
        private DeviceCommunication _comm = DeviceCommunication.Instance;
        private DatabaseManager _dbManager = DatabaseManager.Instance;
        private System.Windows.Forms.Timer _readTimer;
        private int _currentLanguageId = 0;

        public Service()
        {
            InitializeComponent();
            SetupReadingTimer();
            UpdateLanguage(0);
        }

        public void UpdateLanguage(int langId)
        {
            _currentLanguageId = langId;
            
            if (langId == 1) 
            {
                btnServoMove.Text = "Move Motor";
                btnHomingAll.Text = "Homing All";
                btnCalibration.Text = "Calibrate";
                lblLoadCellTitle.Text = "Weight:";
            }
            else if (langId == 2) 
            {
                btnServoMove.Text = "تحريك المحرك";
                btnHomingAll.Text = "إعادة التعيين";
                btnCalibration.Text = "معايرة";
                lblLoadCellTitle.Text = "وزن:";
            }
            else 
            {
                btnServoMove.Text = "Motoru Hareket Ettir";
                btnHomingAll.Text = "Homing Başlat";
                btnCalibration.Text = "Kalibrasyon";
                lblLoadCellTitle.Text = "Ağırlık:";
            }
        }

        private void btnServoMove_Click(object sender, EventArgs e)
        {
            if (!_comm.IsConnected)
            {
                ShowStatusMessage("Cihaz bağlı değil!", true);
                return;
            }

            try
            {
                if (int.TryParse(txtServoSpeed.Text, out int speed) && int.TryParse(txtServoDistance.Text, out int distance))
                {
                    bool success = _comm.SetServoMotorPosition(0, distance);
                    
                    if (success) 
                    {
                        lblMotorStatus.Text = _currentLanguageId == 0 ? "Komut Gönderildi" : "Command Sent";
                        lblMotorStatus.ForeColor = Color.Green;
                        _dbManager.LogDeviceCommand("SetServoMotor", "SUCCESS");
                    }
                    else 
                    {
                        ShowStatusMessage("Komut başarısız!", true);
                        _dbManager.LogDeviceCommand("SetServoMotor", "FAIL");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Haberleşme Hatası: " + ex.Message);
            }
        }

        private void ReadData_Tick(object sender, EventArgs e)
        {
            if (_comm.IsConnected)
            {
                var packet = _comm.GetLatestLoadCellData();
                if (packet != null)
                {
                    lblLoadCellValue.Text = $"{packet.WeightBalance:F2} kg";
                }

                bool[] switches = _comm.ReadLimitSwitches();
                if (switches != null && switches.Length > 0)
                {
                    lblLSXMin.BackColor = switches[0] ? Color.Green : Color.Red;
                    lblLSXMin.Text = $"X Min {(switches[0] ? "AKTİF" : "PASİF")}";
                }
            }
            else
            {
                lblLoadCellValue.Text = "BAĞLANTI YOK";
            }
        }

        private void SetupReadingTimer()
        {
            _readTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _readTimer.Tick += ReadData_Tick;
            _readTimer.Start();
        }

        private void ShowStatusMessage(string message, bool isError)
        {
            lblMotorStatus.Text = message;
            lblMotorStatus.ForeColor = isError ? Color.Red : Color.Black;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _readTimer?.Stop();
                _readTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}