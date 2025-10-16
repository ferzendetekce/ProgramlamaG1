using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace hastakayit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\iboo\OneDrive\المستندات\hasta kayit.mdf"";Integrated Security=True;Connect Timeout=30");

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }

                Con.Open();

                // استخدام الأسماء الصحيحة من قاعدة البيانات
                SqlCommand cmd = new SqlCommand("INSERT INTO [Table] (ADI, soyad, yas, cinsiyet, email, telefon, adress, kan) VALUES (@ADI, @soyad, @yas, @cinsiyet, @email, @telefon, @adress, @kan)", Con);

                cmd.Parameters.AddWithValue("@ADI", textBox1.Text);
                cmd.Parameters.AddWithValue("@soyad", textBox2.Text);
                cmd.Parameters.AddWithValue("@yas", textBox4.Text);
                cmd.Parameters.AddWithValue("@cinsiyet", comboBox1.Text);
                cmd.Parameters.AddWithValue("@email", textBox5.Text);
                cmd.Parameters.AddWithValue("@telefon", textBox8.Text);
                cmd.Parameters.AddWithValue("@adress", textBox7.Text);
                cmd.Parameters.AddWithValue("@kan", comboBox2.Text);

                cmd.ExecuteNonQuery();
                Con.Close();

                MessageBox.Show("Hasta Kaydı Başarıyla Eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // مسح الحقول
                textBox1.Clear();
                textBox2.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox7.Clear();
                textBox8.Clear();
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata Detayı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Programdan çıkmak istediğinizden emin misiniz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }
    }
}