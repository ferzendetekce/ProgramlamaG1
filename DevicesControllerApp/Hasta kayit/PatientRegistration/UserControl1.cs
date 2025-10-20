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
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;port=5432;Database=Patients;User Id=postgres;Password=123456");



        private void UserControl1_Load(object sender, EventArgs e)
        {

            string query = "select * from \"patients\"";
            NpgsqlDataAdapter dt=new NpgsqlDataAdapter(query,connection);
            DataSet ds=new DataSet();
            dt.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];




        }





    }
}
