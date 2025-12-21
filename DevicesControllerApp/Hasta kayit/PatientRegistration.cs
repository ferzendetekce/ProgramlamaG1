using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevicesControllerApp.Hasta_kayit
{
    public partial class PatientRegistration : UserControl
    {
        Database.DatabaseManager db = Database.DatabaseManager.Instance;
        public PatientRegistration()
        {
            InitializeComponent();
            db.OpenConnection();
           dataGridView1.DataSource= db.GetAllCitys();
            comboBox1.DataSource = db.GetAllCitys();
            comboBox1.DisplayMember = "sehir_adi";
            comboBox1.ValueMember = "plaka_kodu";
        }

        
        private void PatientRegistration_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(" Plaka no:" + comboBox1.SelectedValue.ToString() + "Şehir:"+ comboBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            db.HastaSil(long.Parse(textBox2.Text));
        }
    }
}
