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
        public MainForm()
        {
            InitializeComponent();
        }



        private void btnSettings_Click(object sender, EventArgs e)
        {
            Settings s = new Settings();
            // Make the UserControl fill the entire panel

            s.OnLanguageChanged += ApplyLanguage;
            s.OnThemeChanged += ApplyTheme;


            s.Dock = DockStyle.Fill;
            s.Margin = Padding.Empty;
            splitContainer2.Panel2.Controls.Clear();
            splitContainer2.Panel2.Controls.Add(s);
            s.BringToFront();
        }



        // TÜM UYGULAMA İÇİN DİL DEĞİŞTİRME METODU
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




        // TÜM UYGULAMA İÇİN TEMA DEĞİŞTİRME METODU
        public void ApplyTheme(bool isDark)
        {
            Color panelColor;
            Color buttonColor;
            Color textColor;

            if (isDark) // Koyu Tema
            {
                panelColor = Color.FromArgb(30, 30, 30); // Koyu Gri/Siyah
                buttonColor = Color.FromArgb(50, 50, 50);
                textColor = Color.White;
            }
            else // Açık Tema (Varsayılan renklerinizi buraya yazın)
            {
                panelColor = Color.SandyBrown; // Designer'da görülen renk
                buttonColor = SystemColors.Control; // Veya butonun orijinal rengi
                textColor = Color.Black;
            }

            // 1. Ana Panelleri Boya
            this.BackColor = panelColor;
            splitContainer1.Panel1.BackColor = panelColor; // Sol Menü Paneli
            splitContainer2.Panel1.BackColor = SystemColors.Info; // Üst bilgi paneli (isterseniz bunu da değiştirin)
            splitContainer2.Panel2.BackColor = panelColor; // İçerik paneli

            // 2. Sol Menü Butonlarını Boya
            ChangeButtonTheme(btnTherapy, buttonColor, textColor);
            ChangeButtonTheme(btnPatient, buttonColor, textColor);
            ChangeButtonTheme(btnUsers, buttonColor, textColor);
            ChangeButtonTheme(btnMonitoring, buttonColor, textColor);
            ChangeButtonTheme(btnReports, buttonColor, textColor);
            ChangeButtonTheme(btnService, buttonColor, textColor);
            ChangeButtonTheme(btnSettings, buttonColor, textColor);

            // Alt butonlar (button8, button9...)
            ChangeButtonTheme(button8, buttonColor, textColor);
            ChangeButtonTheme(button9, buttonColor, textColor);
            ChangeButtonTheme(button10, buttonColor, textColor);
            ChangeButtonTheme(button11, buttonColor, textColor);
        }




        // Yardımcı metod: Buton renklerini değiştirmek için
        private void ChangeButtonTheme(Button btn, Color backColor, Color foreColor)
        {
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat; // Daha modern görünüm için
            btn.FlatAppearance.BorderColor = foreColor;
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
    }
}
