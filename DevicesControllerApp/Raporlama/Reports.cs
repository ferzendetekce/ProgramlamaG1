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
using iTextSharp.text.pdf;
using iTextSharp.text;
using Npgsql;
using System.Globalization;
using System.Threading;

namespace DevicesControllerApp.Raporlama
{
    public partial class Reports : UserControl
    {

        private string connectionString =
           "Host=localhost;Port=5432;Database=lokomatDB;Username=postgres;Password=1234;";

        public Reports()
        {
            InitializeComponent();
            FillLanguageCombo();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void FillLanguageCombo()
        {
            cmbLanguage.Items.Clear();
            cmbLanguage.Items.Add("Türkçe");
            cmbLanguage.Items.Add("English");
            cmbLanguage.Items.Add("العربية");

            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void btnRaporOlustur_Click(object sender, EventArgs e)
        {
            string isim = txt_Isim.Text.Trim();
            string soyisim = txt_Soyisim.Text.Trim();
            string tc = txt_TcNo.Text.Trim();

            DateTime baslangicTarihi = dtBaslangic.Value.Date;
            DateTime bitisTarihi = dtBitis.Value.Date.AddDays(1).AddSeconds(-1);


            if(rbHastalariListele.Checked)
            {
                HastalariListele(isim, soyisim, tc, baslangicTarihi, bitisTarihi);

            }

            else if (rbSeanslarıListele.Checked)
            {
                SeanslariListele(isim, soyisim, tc, baslangicTarihi, bitisTarihi);

            }

            else if(rbOperatorleriListele.Checked)
            {
                OperatorListele(isim, soyisim, tc, baslangicTarihi, bitisTarihi);
            }

            else if(rbLoglariListele.Checked)
            {
                LoglariListele(isim, soyisim, tc, baslangicTarihi, bitisTarihi);

            }

            else
            {
                MessageBox.Show("Lütfen rapor türü seçiniz!");
            }

        }

        private void HastalariListele(string isim,string soyisim,string tc,DateTime baslangicTarihi,DateTime bitisTarihi)
        {
           


        }

        private void SeanslariListele(string isim, string soyisim, string tc, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            if (string.IsNullOrWhiteSpace(tc))
            {
                MessageBox.Show("Lütfen TC Kimlik Numarası giriniz.");
                return;
            }

            Database.DatabaseManager db = Database.DatabaseManager.Instance;

            DataTable result = db.GetPatientSessionsByTc(tc);

            if (result.Rows.Count == 0)
            {
                MessageBox.Show("Bu TC Kimlik numarasına ait seans bulunamadı.");
            }

            dataGridView1.DataSource = result;
        }
        

        private void OperatorListele(string isim, string soyisim, string tc, DateTime baslangicTarihi, DateTime bitisTarihi)
        {

        }

        private void LoglariListele(string isim, string soyisim, string tc, DateTime baslangicTarihi, DateTime bitisTarihi)
        {

        }

        private void Reports_Load_1(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode
       == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            try
            {
                using (var conn = new Npgsql.NpgsqlConnection(connectionString))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata:\n" + ex.Message);
            }
        }

        public void SetLanguage(string culture)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            this.Controls.Clear();
            InitializeComponent();
            FillLanguageCombo();
            
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("PDF oluşturmak için listelenmiş veri yok.");
                    return;
                }

                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "PDF Dosyası|*.pdf";
                save.Title = "PDF Kaydet";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Document doc = new Document(PageSize.A4.Rotate());
                        PdfWriter.GetInstance(doc, new FileStream(save.FileName, FileMode.Create));
                        doc.Open();

                        PdfPTable pdfTable = new PdfPTable(dataGridView1.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        // Başlıklar
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            pdfTable.AddCell(cell);
                        }

                        // Satırlar
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.IsNewRow) continue;

                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                pdfTable.AddCell(cell.Value?.ToString() ?? "");
                            }
                        }

                        doc.Add(new Paragraph("Hasta Seans Raporu"));
                        doc.Add(new Paragraph(" "));
                        doc.Add(pdfTable);

                        doc.Close();

                        MessageBox.Show(" PDF başarıyla oluşturuldu.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(" PDF oluşturulurken hata oluştu:\n" + ex.Message);
                    }
                }
            }
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLanguage.SelectedIndex == 0)
                SetLanguage("tr-TR");
            else if (cmbLanguage.SelectedIndex == 1)
                SetLanguage("en-US");
            else if (cmbLanguage.SelectedIndex == 2)
                SetLanguage("ar-SA");
        }
    }
}
