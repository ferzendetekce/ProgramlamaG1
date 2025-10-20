using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatientRegistration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            

        }
        NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;port=5432;Database=Patients;User Id=postgres;Password=123456");

        private void seeAll()
        {
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.ColumnHeadersVisible = true;

            string query = "select * from \"patients\"";
            NpgsqlDataAdapter dt = new NpgsqlDataAdapter(query, connection);
            DataSet ds = new DataSet();
            dt.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Dock = DockStyle.Fill; // formu doldursun
            dataGridView1.ScrollBars = ScrollBars.Both; // hem dikey hem yatay
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AllowUserToAddRows = false; // otomatik boş satır eklemeyi kapat

            seeAll();

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
            try
            {
                // ✅ Önce doğrulama
                if (!ValidateInputs())
                    return;

                connection.Open();

                string query = @"
    INSERT INTO patients (
        tc_no, first_name, last_name, birth_date, gender, address, phone, email, 
        height_cm, weight_kg, foot_size, hip_knee_distance, ankle_heel_distance, 
        diagnosis, treatment_start_date, doctor_note, allergies, chronic_diseases
    )
    VALUES (
        @tc_no, @first_name, @last_name, @birth_date, @gender, @address, @phone, @email,
        @height_cm, @weight_kg, @foot_size, @hip_knee_distance, @ankle_heel_distance,
        @diagnosis, @treatment_start_date, @doctor_note, @allergies, @chronic_diseases
    )";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    // TextBox ve MaskedTextBox değerleri
                    command.Parameters.AddWithValue("@tc_no", tc_no.Text);
                    command.Parameters.AddWithValue("@first_name", textBox_name.Text);
                    command.Parameters.AddWithValue("@last_name", textBox_surname.Text);
                    command.Parameters.AddWithValue("@birth_date", dateTimePicker1.Value.Date);

                    // RadioButton ile cinsiyet
                    string gender = radioButton_man.Checked ? "Erkek" : radioButton_woman.Checked ? "Kadın" : "";
                    command.Parameters.AddWithValue("@gender", gender);

                    command.Parameters.AddWithValue("@address", textBox_adress.Text);
                    command.Parameters.AddWithValue("@phone", maskedTextBox_phone.Text);
                    command.Parameters.AddWithValue("@email", textBox_email.Text);

                    // NumericUpDown değerleri
                    command.Parameters.AddWithValue("@height_cm", numericUpDown_boy.Value);
                    command.Parameters.AddWithValue("@weight_kg", numericUpDown_kilo.Value);
                    command.Parameters.AddWithValue("@foot_size", numericUpDown_ayak.Value);
                    command.Parameters.AddWithValue("@hip_knee_distance", numericUpDown_kalca.Value);
                    command.Parameters.AddWithValue("@ankle_heel_distance", numericUpDown_diz.Value);

                    // Diğer TextBox değerleri
                    command.Parameters.AddWithValue("@diagnosis", textBox_tani.Text);
                    command.Parameters.AddWithValue("@treatment_start_date", dateTimePicker_tedaviBas.Value.Date);
                    command.Parameters.AddWithValue("@doctor_note", textBox_doktorNot.Text);
                    command.Parameters.AddWithValue("@allergies", textBox_alerji.Text);
                    command.Parameters.AddWithValue("@chronic_diseases", textBox_kronikHasta.Text);

                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Kayıt başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                seeAll();
                clearAll();
                connection.Close();
            }
        }

        private void button_remove_Click(object sender, EventArgs e)
        {

            if(textBox_patientId.Text == "")
            {
                MessageBox.Show("Lütfen silinecek hastayı seçiniz.");
                return;
            }
            else
            {
                connection.Open();
                string query = "DELETE FROM patients WHERE patien_id = @id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", int.Parse(textBox_patientId.Text));
                command.ExecuteNonQuery();
                seeAll();
                connection.Close();
            }


        }

        private void button_update_Click(object sender, EventArgs e)
        {
          



            if(textBox_patientId.Text=="")
            {
                MessageBox.Show("Lütfen güncellenecek hastayı seçiniz.","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {

                connection.Open();
                string query = @"
   UPDATE patients SET
       tc_no = @tc_no,
       first_name = @first_name,
       last_name = @last_name,
       birth_date = @birth_date,
       gender = @gender,
       address = @address,
       phone = @phone,
       email = @email,
       height_cm = @height_cm,
       weight_kg = @weight_kg,
       foot_size = @foot_size,
       hip_knee_distance = @hip_knee_distance,
       ankle_heel_distance = @ankle_heel_distance,
       diagnosis = @diagnosis,
       treatment_start_date = @treatment_start_date,
       doctor_note = @doctor_note,
       allergies = @allergies,
       chronic_diseases = @chronic_diseases
   WHERE patien_id = @id";



                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", int.Parse(textBox_patientId.Text));
                command.Parameters.AddWithValue("@tc_no", tc_no.Text);
                command.Parameters.AddWithValue("@first_name", textBox_name.Text);
                command.Parameters.AddWithValue("@last_name", textBox_surname.Text);
                command.Parameters.AddWithValue("@birth_date", dateTimePicker1.Value.Date);
                if (radioButton_man.Checked)
                {
                    command.Parameters.AddWithValue("@gender", "Erkek");
                }
                else
                {
                    command.Parameters.AddWithValue("@gender", "Kadın");
                }

                command.Parameters.AddWithValue("@address", textBox_adress.Text);
                command.Parameters.AddWithValue("@phone", maskedTextBox_phone.Text);
                command.Parameters.AddWithValue("@email", textBox_email.Text);
                command.Parameters.AddWithValue("@height_cm", numericUpDown_boy.Value);
                command.Parameters.AddWithValue("@weight_kg", numericUpDown_kilo.Value);
                command.Parameters.AddWithValue("@foot_size", numericUpDown_ayak.Value);
                command.Parameters.AddWithValue("@hip_knee_distance", numericUpDown_kalca.Value);
                command.Parameters.AddWithValue("@ankle_heel_distance", numericUpDown_diz.Value);
                command.Parameters.AddWithValue("@diagnosis", textBox_tani.Text);
                command.Parameters.AddWithValue("@treatment_start_date", dateTimePicker_tedaviBas.Value.Date);
                command.Parameters.AddWithValue("@doctor_note", textBox_doktorNot.Text);
                command.Parameters.AddWithValue("@allergies", textBox_alerji.Text);
                command.Parameters.AddWithValue("@chronic_diseases", textBox_kronikHasta.Text);
                command.ExecuteNonQuery();
                seeAll();
                connection.Close();


            }









        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            textBox_patientId.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            textBox_name.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBox_surname.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            tc_no.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            dateTimePicker1.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

            if (dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString() == "Erkek")
            {
                radioButton_man.Checked = true;
            }
            else
            {
                radioButton_woman.Checked = true;
            }

            textBox_adress.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            maskedTextBox_phone.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
            textBox_email.Text = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();
            numericUpDown_boy.Value = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString());
            numericUpDown_kilo.Value = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[10].Value.ToString());
            numericUpDown_ayak.Value = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[11].Value.ToString());
           // numericUpDown_kalca.Value = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[12].Value.ToString());
         //   numericUpDown_diz.Value = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[13].Value.ToString());
            textBox_tani.Text = dataGridView1.Rows[e.RowIndex].Cells[14].Value.ToString();
            dateTimePicker_tedaviBas.Text = dataGridView1.Rows[e.RowIndex].Cells[15].Value.ToString();
            textBox_doktorNot.Text = dataGridView1.Rows[e.RowIndex].Cells[16].Value.ToString();
            textBox_alerji.Text = dataGridView1.Rows[e.RowIndex].Cells[17].Value.ToString();
            textBox_kronikHasta.Text = dataGridView1.Rows[e.RowIndex].Cells[18].Value.ToString();







        }

        private void clearAll()
        {
            textBox_adress.Clear();
            textBox_alerji.Clear();
            textBox_doktorNot.Clear();
            textBox_email.Clear();
            textBox_kronikHasta.Clear();
            textBox_name.Clear();
            textBox_patientId.Clear();
            textBox_surname.Clear();
            textBox_tani.Clear();
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
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
          clearAll();




        }

        private void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();

                // Eğer TC alanı boşsa tüm kayıtları getir
                if (string.IsNullOrWhiteSpace(tc_no.Text))
                {
                    seeAll();
                    return;
                }

                // TC'ye göre hasta arama
                string query = "SELECT * FROM patients WHERE tc_no = @tc_no";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tc_no", tc_no.Text);

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    // Eğer hasta bulunursa tabloya yükle
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
