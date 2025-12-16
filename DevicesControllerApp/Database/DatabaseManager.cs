using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq; // List<> kullanabilmek için gerekli
using System.Text;
using System.Threading.Tasks;




namespace DevicesControllerApp.Database
{
    public class DatabaseManager
    {
        // Lütfen buradaki bilgileri kendi PostgreSQL kurulumunuza göre düzenleyin.
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=mydb";

        // Constructor'ı PUBLIC yaptık (Erişim hatası düzeldi)
        public DatabaseManager()
        {

        }

        // Dil Ayarını Güncelleme Metodu
        public bool UpdateLanguage(string languageCode)
        {
            // languageCode: 'tr' veya 'en' gelecek
            string query = "UPDATE general_settings SET application_language = @lang WHERE id = 1";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@lang", languageCode);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata olursa loglayabilir veya mesaj gösterebilirsiniz
                System.Windows.Forms.MessageBox.Show("Dil veritabanına kaydedilemedi: " + ex.Message);
                return false;
            }
        }

        // Tema Ayarını Güncelleme Metodu
        public bool UpdateTheme(string themeName)
        {
            // themeName: 'Light' veya 'Dark' gelecek
            string query = "UPDATE general_settings SET theme = @theme WHERE id = 1";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@theme", themeName);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Tema veritabanına kaydedilemedi: " + ex.Message);
                return false;
            }
        }

        // Diğer metodlar...
        public bool OpenConnection() { return true; } // Basit kontrol için true dönebiliriz şimdilik
        public bool CloseConnection() { return true; }
    }

}
