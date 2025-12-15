using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevicesControllerApp.Ayarlar
{
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();

            // Dil değişimi olayını (Event) bağlıyoruz
            if (comboBox1 != null)
                comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            // Varsayılan dil Türkçe
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lang = comboBox1.SelectedIndex == 1 ? "en" : "tr";
            DiliGuncelle(lang);
        }

        private void DiliGuncelle(string lang)
        {
            if (lang == "en")
            {
                // --- İNGİLİZCE ---
                tabPage1.Text = "General Settings";
                tabPage2.Text = "Device Settings";
                tabPage3.Text = "Security Settings";
                tabPage4.Text = "Database Settings";
                tabPage5.Text = "Mobile Permissions";
                tabPage6.Text = "Email Settings";

                // Genel Ayarlar
                if (UygulamaDiliLbl != null) UygulamaDiliLbl.Text = "Application Language";
                if (TarihSaatLbl != null) TarihSaatLbl.Text = "Date/Time Format";
                if (uzunlukBirimiLbl != null) uzunlukBirimiLbl.Text = "Length Unit";
                if (AğrlıkBirimiLbl != null) AğrlıkBirimiLbl.Text = "Weight Unit";
                if (TemaLbl != null) TemaLbl.Text = "Theme";
                if (Kaydet1Lbl != null) Kaydet1Lbl.Text = "Save";

                // Cihaz Ayarları
                label6.Text = "Min Speed Limits";
                label8.Text = "Max Speed Limits";
                label7.Text = "Timeout (ms)";
                label9.Text = "Auto-Home";
                button2.Text = "Save";

                // Güvenlik
                label10.Text = "Session Timeout";
                label11.Text = "Max Login Attempts";
                label12.Text = "Password Policy";
                button3.Text = "Save";

                // Veritabanı
                label15.Text = "Backup Folder";
                button5.Text = "Select";
                label14.Text = "Log Cleanup";
                label13.Text = "Auto Backup";
                button4.Text = "Save";

                // Mobil
                label21.Text = "Roles";
                label28.Text = "Mobile App Permissions";
                label22.Text = "Admin";
                label23.Text = "Operator";
                label24.Text = "Service";
                label27.Text = "Start Therapy";
                label26.Text = "Stop";
                label25.Text = "Crane Control";
                label30.Text = "Foot Size";
                label29.Text = "Weight Reduction";
                button8.Text = "Save";

                // E-Posta
                label16.Text = "SMTP Server";
                label17.Text = "Sender Email";
                label18.Text = "Password";
                label19.Text = "Port";
                label20.Text = "SSL Enabled";
                button6.Text = "Test";
                button7.Text = "Save";
            }
            else
            {
                // --- TÜRKÇE ---
                tabPage1.Text = "Genel Ayarlar";
                tabPage2.Text = "Cihaz Ayarları";
                tabPage3.Text = "Güvenlik Ayarları";
                tabPage4.Text = "Veritabanı Ayarları";
                tabPage5.Text = "Mobil İzinler";
                tabPage6.Text = "E-Posta Ayarları";

                if (UygulamaDiliLbl != null) UygulamaDiliLbl.Text = "Uygulama Dili";
                if (TarihSaatLbl != null) TarihSaatLbl.Text = "Tarih/Saat";
                if (uzunlukBirimiLbl != null) uzunlukBirimiLbl.Text = "Uzunluk Birimi";
                if (AğrlıkBirimiLbl != null) AğrlıkBirimiLbl.Text = "Ağırlık Birimi";
                if (TemaLbl != null) TemaLbl.Text = "Tema";
                if (Kaydet1Lbl != null) Kaydet1Lbl.Text = "Kaydet";

                label6.Text = "Minimum Hız Limitleri";
                label8.Text = "Maksimum Hız Limitleri";
                label7.Text = "Timeout Süresi (ms)";
                label9.Text = "Otomatik Home";
                button2.Text = "Kaydet";

                label10.Text = "Oturum Timeout Süreleri";
                label11.Text = "Maksimum Hatalı Giriş";
                label12.Text = "Şifre Politikası";
                button3.Text = "Kaydet";

                label15.Text = "Yedekleme Klasörü";
                button5.Text = "Seç";
                label14.Text = "Log Temizleme Politikası";
                label13.Text = "Oto Yedekleme";
                button4.Text = "Kaydet";

                label21.Text = "Roller";
                label28.Text = "Mobil Uygulama İzinleri";
                label22.Text = "Admin";
                label23.Text = "Operatör";
                label24.Text = "Servis";
                label27.Text = "Terapi Başlat";
                label26.Text = "Durdur";
                label25.Text = "Vinç Kontrol";
                label30.Text = "Ayak Numarası Ayarı";
                label29.Text = "Ağırlık Azaltma";
                button8.Text = "Kaydet";

                label16.Text = "SMTP Sunucusu";
                label17.Text = "Gönderen E-Posta";
                label18.Text = "Şifre";
                label19.Text = "Port";
                label20.Text = "SSL Etkin";
                button6.Text = "Test et";
                button7.Text = "Kaydet";
            }
        }
    }
}