using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevicesControllerApp.Hasta_kayit
{
    public partial class PatientRegistration : UserControl
    {
        public PatientRegistration()
        {
            InitializeComponent();
        }
        NpgsqlConnection connection = new NpgsqlConnection(
    "Server=localhost;Port=5432;Database=lokomatDB;User Id=postgres;Password=123456"
     );

        private void seeAll()
        {


            try
            {
                // 2. DEĞİŞİKLİK: Tablo adı 'hastalar' olarak güncellendi.
                string query = "SELECT * FROM hastalar "; // ID'ye göre sıralı gelsin
                NpgsqlDataAdapter dt = new NpgsqlDataAdapter(query, connection);
                DataSet ds = new DataSet();
                dt.Fill(ds);

                dataGridView1.DataSource = ds.Tables[0];

                // DataGridView Ayarları
                //   dataGridView1.Dock = DockStyle.Fill;
                dataGridView1.ColumnHeadersVisible = true;
                dataGridView1.ScrollBars = ScrollBars.Both;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Satırın tamamını seçsin
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekilirken hata oluştu: " + ex.Message);
            }
        }



        private void PatientRegistration_Load(object sender, EventArgs e)
        {
            seeAll();
        }

        private void clearAll()
        {
            textBox_adress.Clear();
            textBox_teshis.Clear();
            textBox_teshisacikalma.Clear();
            textBox_email.Clear();
            textBox_name.Clear();
            textBox_surname.Clear();
            tc_no.Clear();
            maskedTextBox_phone.Clear();
            numericUpDown_ayak.Value = 40;
            numericUpDown_diz.Value = 1;
            numericUpDown_kalca.Value = 5;
            numericUpDown_boy.Value = 100;
            numericUpDown_kilo.Value = 50;
            radioButton_woman.Checked = false;

            radioButton_man.Checked = false;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker_tedaviBas.Value = DateTime.Now;
            textBox_teshis.Clear();
            textBox_teshisacikalma.Clear();

        }

        private bool ValidateInputs()
        {
            // Zorunlu alanlar kontrolü
            if (string.IsNullOrWhiteSpace(tc_no.Text))
            {
                MessageBox.Show("TC kimlik numarası boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox_name.Text))
            {
                MessageBox.Show("Ad alanı boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox_surname.Text))
            {
                MessageBox.Show("Soyad alanı boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!radioButton_man.Checked && !radioButton_woman.Checked)
            {
                MessageBox.Show("Cinsiyet seçimi yapmalısınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox_adress.Text))
            {
                MessageBox.Show("Adres alanı boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(maskedTextBox_phone.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "")))
            {
                MessageBox.Show("Telefon numarası boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox_email.Text))
            {
                MessageBox.Show("E-posta adresi boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Her şey tamam
            return true;
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            // ÖNCE KONTROL ET
            if (KontrolEt() == false)
            {
                // Eğer hata varsa fonksiyondan çık, veritabanı kodlarını çalıştırma
                return;
            }

            connection.Open();
            string query = @"
INSERT INTO hastalar (
    tc_kimlik_no, ad, soyad, dogum_tarihi, cinsiyet_id,
    boy_cm, kilo_kg, bacak_boyu_cm, kalca_diz_boyu_cm, diz_bilek_boyu_cm,
    ayak_numarasi, teshis, teshis_aciklamasi, rahatsizlandigi_tarih,
    telefon, e_posta, adres, sehir_plaka_kodu, olusturulma_tarihi,son_giris_tarihi
)
VALUES (
    @tc, @ad, @soyad, @dogum,
    @cinsiyet,
    @boy, @kilo, @bacak, @kalca, @diz,
    @ayak, @teshis, @aciklama, @rahatsizlikTarihi,
    @telefon, @mail, @adres, @sehir, @olusmatarihi, @sonGiris
)";


            NpgsqlCommand cmd = new NpgsqlCommand(query, connection);



            cmd.Parameters.AddWithValue("@tc", tc_no.Text);
            cmd.Parameters.AddWithValue("@ad", textBox_name.Text);
            cmd.Parameters.AddWithValue("@soyad", textBox_surname.Text);
            cmd.Parameters.AddWithValue("@dogum", dateTimePicker1.Value.Date);

            // cinsiyetler tablosu varsa ID
            cmd.Parameters.AddWithValue("@cinsiyet", radioButton_man.Checked ? 1 : 2);

            cmd.Parameters.AddWithValue("@boy", numericUpDown_boy.Value);
            cmd.Parameters.AddWithValue("@kilo", numericUpDown_kilo.Value);
            cmd.Parameters.AddWithValue("@bacak", numericUpDownbacak.Value);
            cmd.Parameters.AddWithValue("@kalca", numericUpDown_kalca.Value);
            cmd.Parameters.AddWithValue("@diz", numericUpDown_diz.Value);
            cmd.Parameters.AddWithValue("@ayak", numericUpDown_ayak.Value);

            cmd.Parameters.AddWithValue("@teshis", textBox_teshis.Text);
            cmd.Parameters.AddWithValue("@aciklama", textBox_teshisacikalma.Text);
            cmd.Parameters.AddWithValue("@rahatsizlikTarihi", dateTimePicker_tedaviBas.Value.Date);

            cmd.Parameters.AddWithValue("@telefon", maskedTextBox_phone.Text);
            cmd.Parameters.AddWithValue("@mail", textBox_email.Text);
            cmd.Parameters.AddWithValue("@adres", textBox_adress.Text);

            cmd.Parameters.AddWithValue("@sehir", int.Parse(comboBox_sehir.SelectedItem.ToString()));

            cmd.Parameters.AddWithValue("@olusmatarihi", DateTime.Now);
            cmd.Parameters.AddWithValue("@sonGiris", DateTime.Now);

            cmd.ExecuteNonQuery();

            MessageBox.Show("Kayıt başarıyla eklendi!");


            seeAll();
            clearAll();
            connection.Close();
        }

        private void button_remove_Click(object sender, EventArgs e)
        {
            if (tc_no.Text.Length != 11 || !tc_no.Text.All(char.IsDigit))
            {
                MessageBox.Show("TC Kimlik numarası 11 haneli ve sadece rakam olmalıdır!",
                                "Hatalı TC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            else
            {
                connection.Open();
                string query = "DELETE FROM hastalar WHERE tc_kimlik_no = @tc";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@tc", tc_no.Text);
                command.ExecuteNonQuery();
                seeAll();
                connection.Close();
                MessageBox.Show("Kayıt başarıyla silindi!");
            }
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tc_no.Text))
            {
                MessageBox.Show("Lütfen güncellenecek hastayı seçiniz.",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                connection.Open();

                string query = @"
UPDATE hastalar SET
    ad = @ad,
    soyad = @soyad,
    dogum_tarihi = @dogum,
    cinsiyet_id = @cinsiyet,
    boy_cm = @boy,
    kilo_kg = @kilo,
    bacak_boyu_cm = @bacak,
    kalca_diz_boyu_cm = @kalca,
    diz_bilek_boyu_cm = @diz,
    ayak_numarasi = @ayak,
    teshis = @teshis,
    teshis_aciklamasi = @aciklama,
    rahatsizlandigi_tarih = @rahatsizlikTarihi,
    telefon = @telefon,
    e_posta = @mail,
    adres = @adres,
    sehir_plaka_kodu = @sehir,
    son_giris_tarihi = @sonGiris
WHERE tc_kimlik_no = @tc;
";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@tc", tc_no.Text);
                    cmd.Parameters.AddWithValue("@ad", textBox_name.Text);
                    cmd.Parameters.AddWithValue("@soyad", textBox_surname.Text);
                    cmd.Parameters.AddWithValue("@dogum", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@cinsiyet", radioButton_man.Checked ? 1 : 2);

                    cmd.Parameters.AddWithValue("@boy", numericUpDown_boy.Value);
                    cmd.Parameters.AddWithValue("@kilo", numericUpDown_kilo.Value);
                    cmd.Parameters.AddWithValue("@bacak", numericUpDownbacak.Value);
                    cmd.Parameters.AddWithValue("@kalca", numericUpDown_kalca.Value);
                    cmd.Parameters.AddWithValue("@diz", numericUpDown_diz.Value);
                    cmd.Parameters.AddWithValue("@ayak", numericUpDown_ayak.Value);

                    cmd.Parameters.AddWithValue("@teshis", textBox_teshis.Text);
                    cmd.Parameters.AddWithValue("@aciklama", textBox_teshisacikalma.Text);
                    cmd.Parameters.AddWithValue("@rahatsizlikTarihi", dateTimePicker_tedaviBas.Value.Date);

                    cmd.Parameters.AddWithValue("@telefon", maskedTextBox_phone.Text);
                    cmd.Parameters.AddWithValue("@mail", textBox_email.Text);
                    cmd.Parameters.AddWithValue("@adres", textBox_adress.Text);

                    cmd.Parameters.AddWithValue("@sehir", int.Parse(comboBox_sehir.SelectedItem.ToString()));

                    cmd.Parameters.AddWithValue("@sonGiris", DateTime.Now);

                    int affected = cmd.ExecuteNonQuery();

                    if (affected > 0)
                        MessageBox.Show("Kayıt başarıyla güncellendi!");
                    else
                        MessageBox.Show("Güncellenecek kayıt bulunamadı!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                connection.Close();
                seeAll();
                clearAll();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox_name.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBox_surname.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            tc_no.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            dateTimePicker1.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

            if (dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString() == "1")
            {
                radioButton_man.Checked = true;
            }
            else
            {
                radioButton_woman.Checked = true;
            }

            textBox_adress.Text = dataGridView1.Rows[e.RowIndex].Cells[16].Value.ToString();
            maskedTextBox_phone.Text = dataGridView1.Rows[e.RowIndex].Cells[14].Value.ToString();
            textBox_email.Text = dataGridView1.Rows[e.RowIndex].Cells[15].Value.ToString();
            numericUpDown_boy.Value = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString());
            numericUpDown_kilo.Value = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString());
            numericUpDown_ayak.Value = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[10].Value.ToString());
            numericUpDownbacak.Value = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString());
            numericUpDown_kalca.Value = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString());
            numericUpDown_diz.Value = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString());
            textBox_teshis.Text = dataGridView1.Rows[e.RowIndex].Cells[11].Value.ToString();
            textBox_teshisacikalma.Text = dataGridView1.Rows[e.RowIndex].Cells[12].Value.ToString();
            dateTimePicker_tedaviBas.Text = dataGridView1.Rows[e.RowIndex].Cells[13].Value.ToString();

        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            clearAll();
        }

        private void button_search_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(tc_no.Text))
            {
                MessageBox.Show("Lütfen TC Kimlik numarası giriniz!");
                return;
            }

            try
            {
                connection.Open();

                string query = "SELECT * FROM hastalar WHERE tc_kimlik_no = @tc";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@tc", tc_no.Text);

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Hasta bulunamadı!");
                        dataGridView1.DataSource = null;
                    }
                    else
                    {
                        dataGridView1.DataSource = dt; // ✅ sadece 1 kayıt gelir
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama hatası: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void button_lookAll_Click(object sender, EventArgs e)
        {
            seeAll();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hata almamak için using System.Globalization; ekli olmalı.

            string secilenDil = "";
            if (comboBox1.SelectedItem != null)
                secilenDil = comboBox1.SelectedItem.ToString();

            if (secilenDil == "English" || secilenDil == "EN")
            {
                // --- İNGİLİZCE AYARLARI ---

                // 1. Bilgisayarın o anki kültürünü İngilizce yap
                CultureInfo en = new CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = en;
                System.Threading.Thread.CurrentThread.CurrentUICulture = en;

                // 2. Etiketleri İngilizce yap
                label5.Text = "Select City:";
                label4.Text = "Name:";
                label3.Text = "Surname:";
                label30.Text = "ID / Passport No:";
                label2.Text = "Birth Date:";
                label9.Text = "Gender:";
                label6.Text = "Email:";
                label8.Text = "Address:";
                label7.Text = "Phone Number:";
                label11.Text = "Height (cm):";
                label12.Text = "Weight (kg):";
                label13.Text = "Shoe Size:";
                label20.Text = "Hip-Knee Dist. (cm):";
                label21.Text = "Knee-Heel Dist. (cm):";
                label22.Text = "Diagnosis Description:";
                label23.Text = "Treatment Start:";

                label24.Text = "Diagnosis:";
                label27.Text = "Leg Length:";

                // 3. Radyo Butonları ve Butonlar
                radioButton_man.Text = "Male";
                radioButton_woman.Text = "Female";

                btn_Create.Text = "Save";
                button_remove.Text = "Delete";
                button_update.Text = "Update";
                button_search.Text = "Search";
                button_clear.Text = "Clear";
                button_lookAll.Text = "List All";

                // 4. TARİH FORMATI (KESİN ÇÖZÜM)
                // İngilizce'de standart sayısal format (12/4/2025)
                dateTimePicker1.Format = DateTimePickerFormat.Short;
                dateTimePicker_tedaviBas.Format = DateTimePickerFormat.Short;
            }
            else
            {
                // --- TÜRKÇE AYARLARI ---

                // 1. Bilgisayarın o anki kültürünü Türkçe yap
                CultureInfo tr = new CultureInfo("tr-TR");
                System.Threading.Thread.CurrentThread.CurrentCulture = tr;
                System.Threading.Thread.CurrentThread.CurrentUICulture = tr;

                // 2. Etiketleri Türkçe yap

                label4.Text = "Adı:";
                label5.Text = "Şehir Seç:";
                label3.Text = "Soyadı:";
                label30.Text = "Tc Kimlik Numarası:";
                label2.Text = "Doğum Tarihi:";
                label9.Text = "Cinsiyet:";
                label6.Text = "Mail:";
                label8.Text = "Adresi:";
                label7.Text = "Telefon Numarası:";
                label11.Text = "Boy(cm):";
                label12.Text = "Kilo(kg):";
                label13.Text = "Ayak Numarası(cm):";
                label20.Text = "Kalça-Diz Mesafesi(cm):";
                label21.Text = "Diz-Topuk Mesafesi(cm):";
                label22.Text = "Teşhis Açıklaması:";
                label23.Text = "Tedavi Başlangıcı:";


                label24.Text = "Teşhis:";
                label27.Text = "Bacak Boyu:";

                // 3. Radyo Butonları ve Butonlar
                radioButton_man.Text = "Erkek";
                radioButton_woman.Text = "Kadın";

                btn_Create.Text = "Kaydet";
                button_remove.Text = "Sil";
                button_update.Text = "Güncelle";
                button_search.Text = "Ara";
                button_clear.Text = "Temizle";
                button_lookAll.Text = "Hepsine Bak";

                // 4. TARİH FORMATI (KESİN ÇÖZÜM)
                // Türkçe'de standart sayısal format (04.12.2025)
                dateTimePicker1.Format = DateTimePickerFormat.Short;
                dateTimePicker_tedaviBas.Format = DateTimePickerFormat.Short;
            }
        }
        private string GetErrorMsg(string hataKodu)
        {
            string dil = comboBox1.SelectedItem?.ToString() ?? "TR";
            bool isEng = (dil == "English" || dil == "EN");

            switch (hataKodu)
            {
                case "Required":
                    return isEng ? "This field cannot be empty!" : "Bu alan boş bırakılamaz!";
                case "TC":
                    return isEng ? "ID must be 11 digits!" : "TC Kimlik 11 haneli olmalı!";
                case "Gender":
                    return isEng ? "Please select a gender!" : "Lütfen cinsiyet seçiniz!";
                case "Phone":
                    return isEng ? "Invalid phone number!" : "Geçersiz telefon numarası!";
                default:
                    return "Error";
            }
        }

        private bool KontrolEt()
        {
            bool durum = true;
            errorProvider1.Clear(); // Önceki hataları temizle

            // 1. TC KİMLİK KONTROLÜ (Boş mu? 11 hane mi?)
            if (string.IsNullOrWhiteSpace(tc_no.Text) || tc_no.Text.Length != 11)
            {
                errorProvider1.SetError(tc_no, GetErrorMsg("TC"));
                durum = false;
            }

            // 2. AD KONTROLÜ
            if (string.IsNullOrWhiteSpace(textBox_name.Text))
            {
                errorProvider1.SetError(textBox_name, GetErrorMsg("Required"));
                durum = false;
            }

            // 3. SOYAD KONTROLÜ
            if (string.IsNullOrWhiteSpace(textBox_surname.Text))
            {
                errorProvider1.SetError(textBox_surname, GetErrorMsg("Required"));
                durum = false;
            }

            // 4. TELEFON KONTROLÜ (MaskedTextBox olduğu için doluluğuna bakarız)
            if (!maskedTextBox_phone.MaskCompleted)
            {
                errorProvider1.SetError(maskedTextBox_phone, GetErrorMsg("Phone"));
                durum = false;
            }

            // 5. CİNSİYET KONTROLÜ
            if (radioButton_man.Checked == false && radioButton_woman.Checked == false)
            {
                // Radyo butonların yanına ünlem koyar
                errorProvider1.SetError(radioButton_woman, GetErrorMsg("Gender"));
                durum = false;
            }

            // Eğer 'durum' false ise kayıt işlemi yapılmayacak
            return durum;
        }
    }
}
