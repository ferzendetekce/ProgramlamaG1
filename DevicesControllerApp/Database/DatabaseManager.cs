using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControllerApp.Database
{
    public class DatabaseManager
    {
        // 1. ADIM: Sınıfın kendi türünde statik bir değişken (instance) tutuyoruz.
        private static DatabaseManager _instance;

        // Veritabanı bağlantı cümlesi
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=mydb";

        // 2. ADIM: Constructor'ı PRIVATE yapıyoruz. 
        // Böylece dışarıdan 'new DatabaseManager()' denilerek yeni nesne üretilmesi engellenir.
        private DatabaseManager()
        {
        }

        // 3. ADIM: Dışarıdan erişilecek tek nokta burasıdır.
        // Eğer nesne daha önce oluşturulmamışsa oluşturur, varsa olanı gönderir.
        public static DatabaseManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseManager();
                }
                return _instance;
            }
        }

        // --- Mevcut Metotlarınız (Aynen kalabilir) ---

        // Dil Ayarını Güncelleme Metodu
        public bool UpdateLanguage(string languageCode)
        {
            // Mevcut kodunuzdaki mantık korunuyor
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
                System.Windows.Forms.MessageBox.Show("Dil veritabanına kaydedilemedi: " + ex.Message);
                return false;
            }
        }

        // Tema Ayarını Güncelleme Metodu
        public bool UpdateTheme(string themeName)
        {
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










        // Mevcut dili veritabanından çeken metot
        public string GetLanguage()
        {
            string languageCode = "tr"; // Varsayılan değer
            string query = "SELECT application_language FROM general_settings WHERE id = 1";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            languageCode = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılanı ("tr") döner, isteğe bağlı loglayabilirsiniz.
                System.Diagnostics.Debug.WriteLine("Dil okunamadı: " + ex.Message);
            }

            return languageCode;
        }


        public bool OpenConnection() { return true; }
        public bool CloseConnection() { return true; }
    }
}