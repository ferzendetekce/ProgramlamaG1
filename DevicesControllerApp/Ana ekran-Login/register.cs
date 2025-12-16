using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq; // Karakter kontrolleri (Any) için gerekli
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevicesControllerApp.Ana_ekran_Login
{
    public partial class register : Form
    {
        // Uyarı ikonu için ErrorProvider
        private ErrorProvider errorProvider = new ErrorProvider();

        public register()
        {
            InitializeComponent();

            // Şifre karakterlerini gizle (textBox1: Şifre, textBox2: Tekrar)
            textBox1.PasswordChar = '*';
            textBox2.PasswordChar = '*';

            // Her iki kutuya yazı yazıldığında da kontrol fonksiyonunu tetikle
            textBox1.TextChanged += PasswordValidation_Event;
            textBox2.TextChanged += PasswordValidation_Event;

            // İkonun sürekli yanıp sönmesini kapatır
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        private void register_Load(object sender, EventArgs e)
        {
            // Form yüklenirken yapılacak işlemler
        }

        // Ortak Doğrulama Eventi
        private void PasswordValidation_Event(object sender, EventArgs e)
        {
            string sifre = textBox1.Text;
            string sifreTekrar = textBox2.Text;

            // --- 1. Bölüm: textBox1 (Ana Şifre) Politikası Kontrolü ---
            List<string> eksikler = new List<string>();

            // Kural: En az 6 karakter
            if (sifre.Length < 6)
                eksikler.Add("En az 6 karakter");

            // Kural: En az 1 büyük harf
            if (!sifre.Any(char.IsUpper))
                eksikler.Add("En az 1 büyük harf");

            // Kural: En az 1 rakam
            if (!sifre.Any(char.IsDigit))
                eksikler.Add("En az 1 rakam");

            // Kural: En az 1 sembol (Harf veya rakam olmayan)
            if (!sifre.Any(ch => !char.IsLetterOrDigit(ch)))
                eksikler.Add("En az 1 sembol");

            // Hataları textBox1 yanında göster veya temizle
            if (eksikler.Count > 0)
            {
                string mesaj = "Şifre Politikası:\n- " + string.Join("\n- ", eksikler);
                errorProvider.SetError(textBox1, mesaj);
            }
            else
            {
                errorProvider.SetError(textBox1, ""); // Hata yoksa temizle
            }

            // --- 2. Bölüm: textBox2 (Şifre Tekrar) Eşleşme Kontrolü ---
            // Sadece tekrar kutusuna bir şey yazıldıysa kontrol et
            if (!string.IsNullOrEmpty(sifreTekrar))
            {
                if (sifre != sifreTekrar)
                {
                    errorProvider.SetError(textBox2, "Şifreler birbiriyle eşleşmiyor!");
                }
                else
                {
                    errorProvider.SetError(textBox2, ""); // Eşleştiyse hatayı kaldır
                }
            }
            else
            {
                // Kutucuk boşsa uyarı verme
                errorProvider.SetError(textBox2, "");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm(); 
            mainForm.Show();
            this.Hide();
        }
    }
}