using DevicesControllerApp.Ayarlar;
using DevicesControllerApp.Hasta_kayit;
using DevicesControllerApp.Kullanici;
using DevicesControllerApp.Raporlama;
using DevicesControllerApp.Servis;
using DevicesControllerApp.Terapi;
using DevicesControllerApp.Veri_izleme;
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

        private bool _isDarkMode = false;
        public MainForm()
        {
            InitializeComponent();
        }


        private void btnSettings_Click(object sender, EventArgs e)
        {
            Settings s = new Settings();
            s.OnLanguageChanged += ApplyLanguage;
            s.OnThemeChanged += ApplyTheme;

            // Ayarlar sayfası açılırken mevcut temayı ona da uygula (veya kontrol et)
            // Not: Settings kendi içinde Load'da varsayılanı seçiyor olabilir, 
            // ama arka plan rengini buradan da garantiye alabiliriz.

            if (_isDarkMode) s.BackColor = Color.FromArgb(45, 45, 48);

            YukleSayfa(s);


            s.ApplyTheme(_isDarkMode);

        }

        // Sayfa yükleme işlemini tek bir metodda toplayalım (Kod tekrarını önlemek için)
        private void YukleSayfa(UserControl s)
        {
            s.Dock = DockStyle.Fill;
            s.Margin = Padding.Empty;


            // --- YENİ EKLENEN KISIM ---
            // Yeni açılan sayfanın arka planını mevcut temaya göre ayarla
            if (_isDarkMode)
            {
                s.BackColor = Color.FromArgb(30, 30, 30); // Koyu renk
                s.ForeColor = Color.White; // Yazı rengi
            }
            else
            {
                s.BackColor = SystemColors.Control; // veya WhiteSmoke
                s.ForeColor = Color.Black;
            }
            // ---------------------------

            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
            s.BringToFront();
        }

        // TEMA DEĞİŞTİRME METODU
        public void ApplyTheme(bool isDark)
        {
            // Seçimi hafızaya kaydet
            _isDarkMode = isDark;

            Color panelColor;
            Color buttonColor;
            Color textColor;
            Color contentColor; // Panel 2 ve içerik rengi

            if (isDark)
            {
                panelColor = Color.FromArgb(45, 45, 48); // Menüler için Koyu Gri
                contentColor = Color.FromArgb(30, 30, 30); // İçerik için Daha Koyu
                buttonColor = Color.Gray;
                textColor = Color.White;
            }
            else
            {
                panelColor = Color.SandyBrown;
                contentColor = SystemColors.Control; // Varsayılan Windows rengi
                buttonColor = SystemColors.Control;
                textColor = Color.Black;
            }

            // 1. Ana Panelleri Boya
            this.BackColor = panelColor;
            splitContainer1.Panel1.BackColor = panelColor;
            splitContainer2.Panel1.BackColor = isDark ? panelColor : SystemColors.Info;
            splitContainer2.Panel2.BackColor = contentColor; // Panel 2 rengi değişiyor

            // 2. Eğer Panel 2'de halihazırda açık bir sayfa varsa onun da rengini hemen değiştir
            if (splitContainer2.Panel2.Controls.Count > 0)
            {
                Control aktifSayfa = splitContainer2.Panel2.Controls[0];
                aktifSayfa.BackColor = contentColor;
                aktifSayfa.ForeColor = textColor;
            }

            // 3. Butonları Boya
            ChangeButtonTheme(btnTherapy, buttonColor, textColor);
            ChangeButtonTheme(btnPatient, buttonColor, textColor);
            ChangeButtonTheme(btnUsers, buttonColor, textColor);
            ChangeButtonTheme(btnMonitoring, buttonColor, textColor);
            ChangeButtonTheme(btnReports, buttonColor, textColor);
            ChangeButtonTheme(btnService, buttonColor, textColor);
            ChangeButtonTheme(btnSettings, buttonColor, textColor);

            ChangeButtonTheme(button8, buttonColor, textColor);
            ChangeButtonTheme(button9, buttonColor, textColor);
            ChangeButtonTheme(button10, buttonColor, textColor);
            ChangeButtonTheme(button11, buttonColor, textColor);
        }



        private void ChangeButtonTheme(Button btn, Color backColor, Color foreColor)
        {
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = foreColor;
        }

        // DİĞER BUTONLARI GÜNCELLEME
        // Artık hepsi "YukleSayfa" metodunu kullandığı için tema otomatik uygulanacak.

        private void btnPatient_Click(object sender, EventArgs e)
        {
            YukleSayfa(new DevicesControllerApp.Hasta_kayit.PatientRegistration());
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            YukleSayfa(new DevicesControllerApp.Raporlama.Reports());
        }

        private void btnService_Click(object sender, EventArgs e)
        {
            YukleSayfa(new DevicesControllerApp.Servis.Service());
        }

        private void btnMonitoring_Click(object sender, EventArgs e)
        {
            YukleSayfa(new DevicesControllerApp.Veri_izleme.DataMonitoring());
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            YukleSayfa(new DevicesControllerApp.Kullanici.UserRegistration());
        }

        private void btnTherapy_Click(object sender, EventArgs e)
        {
            YukleSayfa(new DevicesControllerApp.Terapi.Therapy());
        }








        public void ApplyLanguage(string lang)
        {
            if (lang == "en")
            {
                // Sol Menü Butonları - İngilizce
                btnTherapy.Text = "THERAPY";
                btnPatient.Text = "PATIENT REGISTRATION";
                btnUsers.Text = "USER REGISTRATION";
                btnMonitoring.Text = "DATA MONITORING";
                btnReports.Text = "REPORTS";
                btnService.Text = "SERVICE";
                btnSettings.Text = "SETTINGS";

                // Buraya Main form üzerindeki diğer labelları da ekleyebilirsiniz.
            }
            else
            {
                // Sol Menü Butonları - Türkçe
                btnTherapy.Text = "TERAPİ";
                btnPatient.Text = "HASTA KAYIT";
                btnUsers.Text = "KULLANICI KAYIT";
                btnMonitoring.Text = "REHABİLİTASYON İZLEME";
                btnReports.Text = "RAPORLAMA";
                btnService.Text = "SERVİS";
                btnSettings.Text = "AYARLAR";
            }
        }
    }
}
