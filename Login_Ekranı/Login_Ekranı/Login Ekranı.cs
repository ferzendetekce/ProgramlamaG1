using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json;


namespace Login_Ekranı
{
    public partial class Form1 : Form
    {
        private Size originalFormSize;
        private Dictionary<Control, Rectangle> originalControlBounds;
        private string secilenDil = "türkçe";

        private string jsonData = @"
        {
          ""türkçe"": {
            ""kullanici_adi"": ""Kullanıcı Adı"",
            ""sifre"": ""Şifre"",
            ""giris"": ""Giriş"",
            ""cikis"": ""Çıkış"",
            ""beni_hatirla"": ""Beni Hatırla"",
            ""sifre_sifirlama2"": ""Şifremi Unuttum"",
            ""sifre_goster"": ""Şifreyi Göster"",
            ""bos_kulanici_adi"": ""Kullanıcı adınızı girmediniz."",
            ""bos_sifre"": ""Şifrenizi girmediniz."",
            ""bos_kullanici_adi_ve_sifre"": ""Kullanıcı adınızı ve şifrenizi girmediniz."",
            ""yanlis_kullanici_adi"": ""Kullanıcı adınızı yanlış girdiniz."",
            ""yanlis_sifre"": ""Şifrenizi yanlış girdiniz."",
            ""yanlis_kullanici_adi_ve_sifre"": ""Kullanıcı adınızı ve şifrenizi yanlış girdiniz."",
            ""uc_kez_hatali_giris"":""3 kez yanlış girdiniz. Lütfen 30 saniye bekleyin."",
            ""cikis_sor"": ""Uygulamadan çıkmak istiyor musunuz?"",
            ""cikis"": ""Çıkış"",
            ""sifre_sifirlama"":  ""Şifre sıfırlama işlemi burada yapılacaktır."",
            ""bilgi"": ""Bilgi"",
            ""bekleme_mesaji"": ""Lütfen {0} saniye bekleyin...""
          },
          ""ingilizce"": {
            ""kullanici_adi"": ""Username"",
            ""sifre"": ""Password"",
            ""giris"": ""Login"",
            ""cikis"": ""Exit"",
            ""beni_hatirla"": ""Remember Me"",
            ""sifre_sifirlama2"": ""Forgot Password"",
            ""sifre_goster"": ""Show Password"",
            ""bos_kullanici_adi"": ""You didn't enter your username."",
            ""bos_sifre"": ""You didn't enter your password."",
            ""bos_kullanici_adi_ve_sifre"": ""You didn't enter your username and password."",
            ""yanlis_kullanici_adi"": ""Wrong username."",
            ""yanlis_sifre"": ""Wrong password."",
            ""yanlis_kullanici_adi_ve_sifre"": ""Wrong username and password."",
            ""uc_kez_hatali_giris"": ""3 wrong attempts. Please wait 30 seconds."",
            ""cikis_sor"": ""Do you want to exit the application?"",
            ""cikis"": ""Exit"",
            ""sifre_sifirlama"": ""Password reset will be done here."",
            ""bilgi"": ""Information"",
            ""bekleme_mesaji"": ""Please wait {0} seconds...""
          },
            ""fransızca"": {
            ""kullanici_adi"": ""Nom d'utilisateur"",
            ""sifre"": ""Mot de passe"",
            ""giris"": ""Connexion"",
            ""cikis"": ""Déconnexion"",
            ""beni_hatirla"": ""Se souvenir de moi"",
            ""sifre_sifirlama2"": ""Mot de passe oublié"",
            ""sifre_goster"": ""Afficher le mot de passe"",
            ""bos_kullanici_adi"": ""Vous n'avez pas saisi votre nom d'utilisateur."",
            ""bos_sifre"": ""Vous n'avez pas saisi votre mot de passe."",
            ""bos_kullanici_adi_ve_sifre"": ""Vous n'avez pas saisi votre nom d'utilisateur et votre mot de passe."",
            ""yanlis_kullanici_adi"": ""Nom d'utilisateur incorrect."",
            ""yanlis_sifre"": ""Mot de passe incorrect."",
            ""yanlis_kullanici_adi_ve_sifre"": ""Nom d'utilisateur et mot de passe incorrects."",
            ""uc_kez_hatali_giris"": ""3 tentatives incorrectes. Veuillez patienter 30 secondes."",
            ""cikis_sor"": ""Voulez-vous quitter l'application ?"",
            ""cikis"": ""Quitter"",
            ""sifre_sifirlama"": ""La réinitialisation du mot de passe sera effectuée ici."",
            ""bilgi"": ""Information"",
            ""bekleme_mesaji"": ""Veuillez patienter {0} secondes...""
          },
          ""çince"": {
            ""kullanici_adi"": ""用户名"",
            ""sifre"": ""密码"",
            ""giris"": ""登录"",
            ""cikis"": ""退出"",
            ""beni_hatirla"": ""记住我"",
            ""sifre_sifirlama2"": ""忘记密码"",
            ""sifre_goster"": ""显示密码"",
            ""bos_kullanici_adi"": ""您未输入用户名。"",
            ""bos_sifre"": ""您未输入密码。"",
            ""bos_kullanici_adi_ve_sifre"": ""您未输入用户名和密码。"",
            ""yanlis_kullanici_adi"": ""用户名错误。"",
            ""yanlis_sifre"": ""密码错误。"",
            ""yanlis_kullanici_adi_ve_sifre"": ""用户名和密码错误。"",
            ""uc_kez_hatali_giris"": ""错误尝试3次。请等待30秒。"",
            ""cikis_sor"": ""您要退出应用程序吗？"",
            ""cikis"": ""退出"",
            ""sifre_sifirlama"": ""密码重置将在此进行。"",
            ""bilgi"": ""信息"",
            ""bekleme_mesaji"": ""请等待 {0} 秒...""
          }
        }";

        private Dictionary<string, Dictionary<string, string>> diller;

        string kullanici_adi; string kullanici_adi2; string sifre; string sifre2; int hatali_giris_sayisi; int kalan_sure;
        public Form1()
        {
            InitializeComponent();
            originalFormSize = this.Size;
            originalControlBounds = new Dictionary<Control, Rectangle>();
            foreach (Control c in this.Controls)
            {
                originalControlBounds[c] = c.Bounds;
            }

            // Form resize eventi
            this.Resize += Form1_Resize;

            diller = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);

            // ComboBox dil seçenekleri ekle
            comboBox1.Items.Add("Türkçe");
            comboBox1.Items.Add("English");
            comboBox1.Items.Add("中文");
            comboBox1.Items.Add("Français");
            comboBox1.SelectedIndex = 0; // default Türkçe

            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

            // Başlangıçta dil yükle
            YaziGuncelle("türkçe");
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string secilenDil = "türkçe";

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    secilenDil = "türkçe";
                    break;
                case 1:
                    secilenDil = "ingilizce";
                    break;
                case 2:
                    secilenDil = "çince";
                    break;
                case 3:
                    secilenDil = "fransızca";
                    break;

            }

            YaziGuncelle(secilenDil);
        }

        private void YaziGuncelle(string secilenDil)
        {
            // Arayüz öğelerini güncelle
            label1.Text = diller[secilenDil]["kullanici_adi"];
            label2.Text = diller[secilenDil]["sifre"];
            button1.Text = diller[secilenDil]["giris"];
            button2.Text = diller[secilenDil]["cikis"];
            checkBox2.Text = diller[secilenDil]["beni_hatirla"];
            button3.Text = diller[secilenDil]["sifre_sifirlama2"];
            checkBox1.Text = diller[secilenDil]["sifre_goster"];
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            float xRatio = (float)this.Width / originalFormSize.Width;
            float yRatio = (float)this.Height / originalFormSize.Height;

            foreach (Control c in this.Controls)
            {
                Rectangle r = originalControlBounds[c];
                c.SetBounds(
                    (int)(r.X * xRatio),
                    (int)(r.Y * yRatio),
                    (int)(r.Width * xRatio),
                    (int)(r.Height * yRatio)
                );
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

 


        private async void button1_Click(object sender, EventArgs e)
        {   
            kullanici_adi = "admin";
            sifre = "1234";
            kullanici_adi2 = textBox1.Text;
            sifre2 = textBox2.Text;

            if (string.IsNullOrEmpty(textBox1.Text) && string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show(diller[secilenDil] ["bos_kullanici_adi_ve_sifre"], diller[secilenDil]["bilgi"], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show(diller[secilenDil] ["bos_kullanici_adi"], diller[secilenDil] ["bilgi"],MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show(diller[secilenDil]["bos_sifre"], diller[secilenDil]["bilgi"], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else 
            {
                if (kullanici_adi != kullanici_adi2 && sifre == sifre2)
                {
                    MessageBox.Show(diller[secilenDil]["yanlis_kullanici_adi"], diller[secilenDil]["bilgi"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    hatali_giris_sayisi++;
                }

                if (sifre != sifre2 && kullanici_adi == kullanici_adi2)
                {
                    MessageBox.Show(diller[secilenDil]["yanlis_sifre"], diller[secilenDil]["bilgi"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    hatali_giris_sayisi++;
                }

                if (kullanici_adi != kullanici_adi2 && sifre != sifre2)
                {
                    MessageBox.Show(diller[secilenDil]["yanlis_kullanici_adi_ve_sifre"], diller[secilenDil]["bilgi"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    hatali_giris_sayisi++;
                }
            }
            if (hatali_giris_sayisi == 3)
            {
                

                //await Task.Delay(10000); // 3 saniye bekle

                // GIF ve yazıyı gizle
                //pictureBox1.Visible = false;

                MessageBox.Show(diller[secilenDil]["uc_kez_hatali_giris"]);
                button1.Enabled = false; // Butonu devre dışı bırak
                int kalan_sure = 30; // 30 saniye

                label3.Visible = true; // Label görünür yap
                label3.Text = string.Format(diller[secilenDil]["bekleme_mesaji"], kalan_sure);
                pictureBox1.Image = Image.FromFile("timer.gif");
                pictureBox1.Visible = true;

                Timer timer = new Timer();
                timer.Interval = 1000; // 1 saniye aralıklarla
                timer.Tick += (s, ev) =>
                {
                    kalan_sure--;
                    label3.Text = string.Format(diller[secilenDil]["bekleme_mesaji"], kalan_sure);

                    if (kalan_sure < 0)
                    {
                        timer.Stop();
                        button1.Enabled = true;
                        hatali_giris_sayisi = 0;
                        label3.Visible = false;
                        pictureBox1.Visible = false;
                    }
                };
                timer.Start();

            }






            if (checkBox2.Checked)
            {
                if (kullanici_adi2 == kullanici_adi && sifre == sifre2)
                {
                    Properties.Settings.Default.kullanici_adi = textBox1.Text;
                    Properties.Settings.Default.Save();
                }
            }

            else
            {
                Properties.Settings.Default.kullanici_adi = "";
                Properties.Settings.Default.Save();
            }

            if (kullanici_adi2 == kullanici_adi && sifre2 == sifre)
            {
                // Giriş formunu gizle
                this.Hide();

                // Form3 aç
                Form3 form3 = new Form3();
                form3.Show();

                // Form3’te 10 saniye bekle
                await Task.Delay(10000);

                // Form3’ü kapat
                form3.Close();
    

                // Form2 aç (ana ekran)
                Form2 form2 = new Form2();
                form2.Show();
            }

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(Properties.Settings.Default.kullanici_adi))
            {
                textBox1.Text = Properties.Settings.Default.kullanici_adi;  
                checkBox2.Checked = true;
            }
        }


        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void kullanici_adi_girme_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Properties.Settings.Default.kullanici_adi = textBox1.Text;
            }
            else
            {
                Properties.Settings.Default.kullanici_adi = "";
            }
            Properties.Settings.Default.Save();

            DialogResult cevap = MessageBox.Show(diller[secilenDil]["cikis_sor"], diller[secilenDil]["cikis"], MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(diller[secilenDil]["sifre_sifirlama"], diller[secilenDil]["bilgi"], MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Console.WriteLine(hatali_giris_sayisi);
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: secilenDil = "türkçe"; break;
                case 1: secilenDil = "ingilizce"; break;
                case 2: secilenDil = "çince"; break;
                case 3: secilenDil = "fransızca"; break;
            }

            YaziGuncelle(secilenDil);
        }
    }
}
