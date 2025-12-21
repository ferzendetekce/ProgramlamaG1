using System;
using System.Windows.Forms;
using System.Drawing; 
using DevicesControllerApp.Core;
using System.Collections.Generic;
using DevicesControllerApp.Database;

namespace DevicesControllerApp.Servis
{
    public partial class Service : UserControl
    {
        private DeviceManager _deviceManager;
        private DatabaseManager _dbManager; 
        private System.Windows.Forms.Timer _readTimer; 
        private int _currentLanguageId = 0;

        public Service()
        {
            InitializeComponent();
            _deviceManager = new DeviceManager();
            _dbManager = new DatabaseManager(); 
            
            TryConnectDevices();
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
            if (!_deviceManager.IsConnected)
            {
                ShowStatusMessage("Cihaz bağlı değil! / Device not connected!", true);
                return;
            }

            try
            {
                if (int.TryParse(txtServoSpeed.Text, out int speed) && int.TryParse(txtServoDistance.Text, out int distance))
                {
                    string result = _deviceManager.MoveMotorManual("Servo_1", speed, distance);
                    
                    if (result == "SUCCESS") {
                        lblMotorStatus.Text = _currentLanguageId == 0 ? "İşlem Başarılı" : "Success";
                        lblMotorStatus.ForeColor = Color.Green;
                    }
                    else if (result == "TIMEOUT") {
                        ShowStatusMessage("Zaman aşımı hatası! (Timeout Error)", true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("COM Port Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowStatusMessage(string message, bool isError)
        {
            lblMotorStatus.Text = message;
            lblMotorStatus.ForeColor = isError ? Color.Red : Color.Black;
        }

        private void TryConnectDevices()
        {
            try
            {
                if (_deviceManager.Connect("COM3")) 
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cihaz bağlantısı kurulamadı: " + ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SetupReadingTimer()
        {
            _readTimer = new System.Windows.Forms.Timer();
            _readTimer.Interval = 500; 
            _readTimer.Tick += ReadData_Tick;
            _readTimer.Start();
        }

        private void ReadData_Tick(object sender, EventArgs e)
        {
            if (_deviceManager.IsConnected)
            {
                double loadValue = _deviceManager.ReadLoadCellValue();
                lblLoadCellValue.Text = $"{loadValue:F2} kg";
                UpdateLimitSwitchDisplay(_deviceManager.GetLimitSwitchStatus());
            }
            else
            {
                lblLoadCellValue.Text = "BAĞLANTI YOK";
            }
        }
        
        private void UpdateLimitSwitchDisplay(Dictionary<string, bool> statuses)
        {
            if (statuses.ContainsKey("X_MIN"))
            {
                bool isActive = statuses["X_MIN"];
                lblLSXMin.BackColor = isActive ? Color.Green : Color.Red;
                lblLSXMin.Text = $"X Min {(isActive ? "AKTİF" : "PASİF")}";
            }
            
            if (statuses.ContainsKey("X_MAX"))
            {
                bool isActive = statuses["X_MAX"];
                lblLSXMax.BackColor = isActive ? Color.Green : Color.Red;
                lblLSXMax.Text = $"X Max {(isActive ? "AKTİF" : "PASİF")}";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _readTimer?.Stop();
                _readTimer?.Dispose();
                _deviceManager?.Dispose();
                
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}