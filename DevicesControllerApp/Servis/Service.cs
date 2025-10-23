using System;
using System.Windows.Forms;
using System.Drawing; 
using DevicesControllerApp.Core; // DeviceManager için
using System.Collections.Generic;

namespace DevicesControllerApp.Servis
{
    // Sınıf Adı: Service
    public partial class Service : UserControl
    {
        private DeviceManager _deviceManager;
        private System.Windows.Forms.Timer _readTimer; 

        public Service()
        {
            InitializeComponent();
            
            _deviceManager = new DeviceManager();
            TryConnectDevices();
            
            SetupReadingTimer(); 
        }

        // Cihaz Bağlantısını Yöneten Metot
        private void TryConnectDevices()
        {
            try
            {
                if (_deviceManager.Connect("COM3")) 
                {
                    // Bağlantı başarılıysa burada bir loglama yapılabilir.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cihaz bağlantısı kurulamadı: " + ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // LoadCell ve Limit Switch Anlık Okuma için Timer Kurulumu
        private void SetupReadingTimer()
        {
            _readTimer = new System.Windows.Forms.Timer();
            _readTimer.Interval = 500; 
            _readTimer.Tick += ReadData_Tick;
            _readTimer.Start();
        }

        // Timer her tick ettiğinde çalışacak metot
        private void ReadData_Tick(object sender, EventArgs e)
        {
            if (_deviceManager.IsConnected)
            {
                // LoadCell anlık okuma
                double loadValue = _deviceManager.ReadLoadCellValue();
                lblLoadCellValue.Text = $"{loadValue:F2} kg";
                
                // Limit Switch durumlarını okuma
                UpdateLimitSwitchDisplay(_deviceManager.GetLimitSwitchStatus());
            }
            else
            {
                lblLoadCellValue.Text = "BAĞLANTI YOK";
            }
        }
        
        // Limit Switch Arayüzünü Güncelleyen Metot
        private void UpdateLimitSwitchDisplay(Dictionary<string, bool> statuses)
        {
            // X Min Limit Switch'i güncelleme
            if (statuses.ContainsKey("X_MIN"))
            {
                bool isActive = statuses["X_MIN"];
                lblLSXMin.BackColor = isActive ? Color.Green : Color.Red;
                lblLSXMin.Text = $"X Min {(isActive ? "AKTİF" : "PASİF")}";
            }
            
            // X Max Limit Switch'i güncelleme
            if (statuses.ContainsKey("X_MAX"))
            {
                bool isActive = statuses["X_MAX"];
                lblLSXMax.BackColor = isActive ? Color.Green : Color.Red;
                lblLSXMax.Text = $"X Max {(isActive ? "AKTİF" : "PASİF")}";
            }
        }
        
        /* --- OLAY İŞLEYİCİLERİ --- */

        // Manuel Motor Kontrolü
        private void btnServoMove_Click(object sender, EventArgs e)
        {
            if (!_deviceManager.IsConnected)
            {
                MessageBox.Show("Cihaz bağlı değil!", "Hata");
                return;
            }

            try
            {
                if (int.TryParse(txtServoSpeed.Text, out int speed) && int.TryParse(txtServoDistance.Text, out int distance))
                {
                    _deviceManager.MoveMotorManual("Servo_1", speed, distance);
                    lblMotorStatus.Text = $"Servo 1: Hız={speed}, Mesafe={distance} ile hareket başladı.";
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli sayısal değerler girin.", "Giriş Hatası");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Motor hareketinde hata: " + ex.Message, "İletişim Hatası");
            }
        }

        // Homing (Sıfırlama) İşlemi
        private void btnHomingAll_Click(object sender, EventArgs e)
        {
            if (!_deviceManager.IsConnected)
            {
                MessageBox.Show("Cihaz bağlı değil!", "Hata");
                return;
            }
            try
            {
                _deviceManager.StartHoming("ALL_AXES"); 
                MessageBox.Show("Tüm eksenlerde sıfırlama (Homing) başlatıldı.", "İşlem Başlatıldı");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Homing işleminde hata: " + ex.Message, "İletişim Hatası");
            }
        }

        // Kalibrasyon İşlemi
        private void btnCalibration_Click(object sender, EventArgs e)
        {
            if (!_deviceManager.IsConnected)
            {
                MessageBox.Show("Cihaz bağlı değil!", "Hata");
                return;
            }

            try
            {
                _deviceManager.StartCalibration("LOADCELL_CALIBRATION");
                MessageBox.Show("Sistem kalibrasyonu başlatıldı.", "İşlem Başlatıldı");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kalibrasyon işleminde hata: " + ex.Message, "İletişim Hatası");
            }
        }
        
        // Kaynak Temizleme (Dispose)
        protected override void Dispose(bool disposing)
        {
            // Timer'ı ve DeviceManager'ı temizle
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