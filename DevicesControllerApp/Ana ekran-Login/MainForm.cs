using DevicesControllerApp.Ayarlar;
using DevicesControllerApp.Hasta_kayit;
using DevicesControllerApp.Kullanici;
using DevicesControllerApp.Raporlama;
using DevicesControllerApp.Servis;
using DevicesControllerApp.Terapi;
using DevicesControllerApp.Veri_izleme;
using RehabilitationSystem.Communication;
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
        public MainForm()
        {
            InitializeComponent();
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

        private void button9_Click(object sender, EventArgs e)
        {
            var device = DeviceCommunication.Instance;

            // Port listesini al
            string[] ports = device.GetAvailablePorts();

            if (ports.Length == 0)
            {
                MessageBox.Show("Hiç COM port bulunamadı!", "Test", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string portList = string.Join("\n", ports);
                MessageBox.Show($"Bulunan portlar:\n{portList}", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Port durumunu kontrol et
            bool isOpen = device.IsPortOpen();
            MessageBox.Show($"Port durumu: {(isOpen ? "Açık" : "Kapalı")}", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var device = DeviceCommunication.Instance;

            // Test verisi oluştur
            byte[] testData = { 0x01, 0x02, 0x03, 0x04, 0x05 };

            // CRC16 hesapla
            ushort crc16 = device.CalculateCRC16(testData);

            // Checksum hesapla
            byte checksum = device.CalculateChecksum(testData);

            // Sonuçları göster
            string result = $"Test Verisi: 01 02 03 04 05\n\n";
            result += $"CRC16: 0x{crc16:X4} (Decimal: {crc16})\n";
            result += $"Checksum: 0x{checksum:X2} (Decimal: {checksum})\n\n";

            // Doğrulama testi
            bool crcValid = device.VerifyCRC16(testData, crc16);
            bool checksumValid = device.VerifyChecksum(testData, checksum);

            result += $"CRC16 Doğrulama: {(crcValid ? "✓ BAŞARILI" : "✗ HATALI")}\n";
            result += $"Checksum Doğrulama: {(checksumValid ? "✓ BAŞARILI" : "✗ HATALI")}";

            MessageBox.Show(result, "CRC Test Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Yanlış CRC ile test
            bool wrongCrcTest = device.VerifyCRC16(testData, 0x0000);
            MessageBox.Show($"Yanlış CRC ile test: {(wrongCrcTest ? "HATALI!" : "Doğru çalışıyor ✓")}",
                            "Negatif Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            
        }
    }
}
