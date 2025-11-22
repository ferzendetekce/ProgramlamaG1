using DevicesControllerApp.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevicesControllerApp.Servis
{
    public partial class Service : UserControl
    {
        public Service()
        {
            InitializeComponent();
        }

        private void btnLogGetir_Click(object sender, EventArgs e)
        {
         /*   // 1. Filtreleme değerlerini arayüzden okuyun
            DateTime baslangic = dtpLogBaslangic.Value;
            DateTime bitis = dtpLogBitis.Value;
            string aramaMetni = txtLogArama.Text;

            string logTipi = "";
            if (cmbLogTipi.SelectedItem != null) { logTipi = cmbLogTipi.SelectedItem.ToString(); }
            else
            {
                MessageBox.Show("Lütfen bir log tipi seçin.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string seviye = "Tümü";
            if (cmbLogSeviye.SelectedItem != null) { seviye = cmbLogSeviye.SelectedItem.ToString(); }
            if (seviye == "Tümü") { seviye = null; } // Veritabanı "Tümü" yerine null (boş) bekler.

            try
            {
                DataTable logListesi = null; // Sonucu tutmak için bir DataTable oluşturuyoruz.

                if (logTipi == "Sistem Logları")
                {
                    // DatabaseManager'dan doğru metodu çağırıyoruz.
                    // logListesi = DatabaseManager.GetSystemLogs(baslangic, bitis, null, seviye, aramaMetni);
                    logListesi = DatabaseManager.GetSystemLogs(baslangic, bitis, null, seviye);
                }
                else // "Cihaz Durum Logları"
                {
                    // DatabaseManager'dan diğer log metodu çağırıyoruz.
                    // logListesi = DatabaseManager.GetDeviceStatusLogs(baslangic, bitis, aramaMetni);
                    logListesi = DatabaseManager.GetDeviceStatusLogs(baslangic, bitis);
                }

                // 3. Veriyi DataGridView'e yükleyin
                dgvLoglar.DataSource = logListesi;

                // 4. (Opsiyonel) Kolonları güzelleştirme
                if (dgvLoglar.Columns.Contains("operationDetail"))
                {
                    dgvLoglar.Columns["operationDetail"].HeaderText = "İşlem Detayı";
                    dgvLoglar.Columns["operationDetail"].Width = 400;
                }
                if (dgvLoglar.Columns.Contains("Detay"))
                {
                    dgvLoglar.Columns["Detay"].Width = 400;
                }
                if (dgvLoglar.Columns.Contains("Tarih"))
                {
                    dgvLoglar.Columns["Tarih"].Width = 150;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loglar getirilirken bir hata oluştu: {ex.Message}",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void dgvLoglar_SelectionChanged(object sender, EventArgs e)
        {
            // 1. DataGridView'de seçili bir satır var mı diye kontrol et
            if (dgvLoglar.SelectedRows.Count > 0)
            {
                // 2. Seçili olan ilk satırı al (genellikle sadece 1 satır seçilir)
                DataGridViewRow seciliSatir = dgvLoglar.SelectedRows[0];

                // 3. Detay kutusuna yazmak için bir metin oluşturucu (StringBuilder) kullanalım.
                // Bu, string birleştirmekten daha verimlidir.
                StringBuilder detay = new StringBuilder();

                // 4. Seçili satırdaki tüm hücreleri (kolonları) tek tek dön
                foreach (DataGridViewCell cell in seciliSatir.Cells)
                {
                    // O hücrenin ait olduğu kolonun başlığını al (örn: "Tarih", "Seviye")
                    string kolonBasligi = dgvLoglar.Columns[cell.ColumnIndex].HeaderText;

                    // Hücrenin içindeki değeri al (eğer boşsa "NULL" yaz)
                    string hucreDegeri = cell.Value?.ToString() ?? "NULL";

                    // Detay metnine "Başlık: Değer" şeklinde ekle
                    detay.AppendLine($"[{kolonBasligi}]:");
                    detay.AppendLine(hucreDegeri);
                    detay.AppendLine("--------------------"); // Ayırıcı çizgi
                }

                // 5. Oluşturduğumuz tüm metni detay kutusuna (txtLogDetay) ata
                txtLogDetay.Text = detay.ToString();
            }
            else
            {
                // Eğer seçili satır yoksa (örn: liste boşsa)
                txtLogDetay.Text = "Detayları görmek için listeden bir log satırı seçin.";
            }
        }

        private void btnLogExport_Click(object sender, EventArgs e)
        {
            // 1. Listede (DataGridView) hiç veri var mı diye kontrol et
            if (dgvLoglar.Rows.Count == 0)
            {
                MessageBox.Show("Dışa aktarılacak log bulunamadı. Lütfen önce logları getirin.",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }

            // 2. Kullanıcıya dosyayı nereye kaydedeceğini sormak için bir "Farklı Kaydet" penceresi oluştur
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV Dosyası (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*";
            saveDialog.Title = "Logları CSV Olarak Kaydet";
            saveDialog.FileName = $"LogKayitlari_{DateTime.Now:yyyyMMdd_HHmm}.csv"; // Örnek dosya adı

            // 3. Kullanıcı "Kaydet" butonuna basarsa...
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 4. CSV içeriğini oluşturmak için bir StringBuilder kullan
                    // (Bu, 'string + string' yapmaktan çok daha hızlıdır)
                    StringBuilder csvContent = new StringBuilder();

                    // 5. İlk satıra kolon başlıklarını (Header) ekle
                    // Görünür olan tüm kolonların başlıklarını al
                    IEnumerable<string> kolonBasliklari = dgvLoglar.Columns.Cast<DataGridViewColumn>()
                                                     .Where(col => col.Visible)
                                                     .Select(col => col.HeaderText.Replace(";", ",")); // Başlıkta ; varsa diye garantiye al

                    // Başlıkları ; (noktalı virgül) ile ayırarak ilk satıra ekle
                    csvContent.AppendLine(string.Join(";", kolonBasliklari));

                    // 6. Listede (DataGridView) dönerek her satırın verisini ekle
                    foreach (DataGridViewRow row in dgvLoglar.Rows)
                    {
                        // Yeni satır ekleme satırını (boş olan en son satır) atla
                        if (row.IsNewRow) continue;

                        // O satırdaki görünür hücrelerin değerlerini al
                        IEnumerable<string> hucreDegerleri = row.Cells.Cast<DataGridViewCell>()
                                                .Where(cell => dgvLoglar.Columns[cell.ColumnIndex].Visible)
                                                .Select(cell => (cell.Value?.ToString() ?? "").Replace(";", ",")); // Verinin içinde ; varsa CSV'yi bozmasın

                        // Verileri ; ile ayırarak yeni bir satır olarak ekle
                        csvContent.AppendLine(string.Join(";", hucreDegerleri));
                    }

                    // 7. Oluşturulan tüm metni (CSV içeriğini) dosyaya yaz
                    // Encoding.UTF8 kullanmak Türkçe karakterlerin (ğ, ü, ş, ı, ö, ç) Excel'de bozuk görünmesini engeller
                    System.IO.File.WriteAllText(saveDialog.FileName, csvContent.ToString(), Encoding.UTF8);

                    // 8. Kullanıcıyı bilgilendir
                    MessageBox.Show($"Loglar başarıyla '{saveDialog.FileName}' dosyasına aktarıldı.",
                                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Yazma sırasında bir hata olursa (örn: disk dolu, yetki yok)
                    MessageBox.Show($"Dışa aktarma sırasında bir hata oluştu: {ex.Message}",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // else -> Kullanıcı "İptal"e bastı, hiçbir şey yapma.
        }
    }
}
