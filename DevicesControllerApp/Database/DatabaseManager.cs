using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DevicesControllerApp.Database
{

    internal class DatabaseManager
    {

        private static DatabaseManager _instance;

        public static DatabaseManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DatabaseManager();

                return _instance;
            }
        }



        private DatabaseManager()
        {
            // Connection string yükleme
        }

        public bool OpenConnection()
        {
            return false;
        }

        public bool CloseConnection()
        {
            return false;
        }

        public bool TestConnection()
        {
            return false;
        }

        public bool AddPatient(string tcNo, string firstName, string lastName, DateTime birthDate,
            string gender, string address, string phone, string email, string diagnosis,
            int height, int weight, int shoeSize, int hipKneeDistance, int kneeHeelDistance)
        {
            return false;
        }

        public bool UpdatePatient(int patientId, string tcNo, string firstName, string lastName,
            DateTime birthDate, string gender, string address, string phone, string email,
            string diagnosis, int height, int weight, int shoeSize, int hipKneeDistance,
            int kneeHeelDistance)
        {
            return false;
        }

        public bool DeletePatient(int patientId)
        {
            return false;
        }

        public DataTable SearchPatient(string searchTerm)
        {
            return null;
        }

        public DataTable GetAllPatients()
        {
            return null;
        }

        public DataRow GetPatientDetails(int patientId)
        {
            return null;
        }

        public DataRow GetPatientByTcNo(string tcNo)
        {
            return null;
        }

        public bool ValidateUserLogin(string username, string password)
        {
            return false;
        }

        public DataRow GetUserByUsername(string username)
        {
            return null;
        }

        public bool AddUser(string username, string password, string tcNo, string firstName,
            string lastName, string role, string email, string phone)
        {
            return false;
        }

        public bool UpdateUser(int userId, string username, string tcNo, string firstName,
            string lastName, string role, string email, string phone)
        {
            return false;
        }

        public bool DeleteUser(int userId)
        {
            return false;
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            return false;
        }

        public string GeneratePasswordResetToken(string email)
        {
            return null;
        }

        public bool ResetPasswordWithToken(string token, string newPassword)
        {
            return false;
        }

        public DataTable GetAllUsers()
        {
            return null;
        }

        public bool UpdateLastLogin(int userId)
        {
            return false;
        }

        public int StartTherapy(int patientId, int operatorId, double speed, double weightReduction,
            double supportBarHeight, int shoeSize)
        {
            return 0;
        }

        public bool EndTherapy(int therapyId, string notes)
        {
            return false;
        }

        public bool UpdateTherapy(int therapyId, double speed, double weightReduction,
            double supportBarHeight)
        {
            return false;
        }

        public DataTable SearchTherapy(DateTime? startDate, DateTime? endDate, int? patientId,
            int? operatorId)
        {
            return null;
        }

        public DataRow GetTherapyDetails(int therapyId)
        {
            return null;
        }

        public DataTable GetPatientTherapyHistory(int patientId)
        {
            return null;
        }

        public bool UpdateTherapyStatus(int therapyId, string status)
        {
            return false;
        }

        public bool SaveLoadCellData(int therapyId, DateTime timestamp, double rightHeel,
            double leftHeel, double rightToe, double leftToe, double weightBalance, int index)
        {
            return false;
        }

        public bool SaveLoadCellDataBulk(int therapyId, List<LoadCellData> dataList)
        {
            return false;
        }

        public DataTable GetLoadCellData(int therapyId)
        {
            return null;
        }

        public DataTable GetLoadCellDataByTimeRange(int therapyId, DateTime startTime,
            DateTime endTime)
        {
            return null;
        }

        public bool AddSystemLog(int? userId, string operationType, string operationDetail,
            string ipAddress, string errorLevel)
        {
            return false;
        }

        public bool AddDeviceStatusLog(string servoMotorStatus, string stepMotorStatus,
            string limitSwitchStatus, string errorCodes)
        {
            return false;
        }

        public DataTable GetSystemLogs(DateTime? startDate, DateTime? endDate, int? userId,
            string errorLevel)
        {
            return null;
        }

        public DataTable GetDeviceStatusLogs(DateTime? startDate, DateTime? endDate)
        {
            return null;
        }

        public DataTable GetRecentErrors(int count)
        {
            return null;
        }

        public string GetSetting(string settingKey)
        {
            return null;
        }

        public bool SaveSetting(string settingKey, string settingValue, string description,
            int updatedByUserId)
        {
            return false;
        }

        public DataTable GetAllSettings()
        {
            return null;
        }

        public bool DeleteSetting(string settingKey)
        {
            return false;
        }

        public int GetTherapyCountByDateRange(DateTime startDate, DateTime endDate)
        {
            return 0;
        }

        public DataTable GetTherapyStatisticsByDateRange(DateTime startDate, DateTime endDate)
        {
            return null;
        }

        public DataTable GetPatientTherapyReport(int patientId)
        {
            return null;
        }

        public DataTable GetOperatorPerformanceReport(DateTime startDate, DateTime endDate)
        {
            return null;
        }

        public DataTable GetOperatorTherapyCount(int operatorId, DateTime startDate,
            DateTime endDate)
        {
            return null;
        }

        public DataTable GetAverageTherapyDuration(DateTime startDate, DateTime endDate)
        {
            return null;
        }

        public DataTable GetPatientProgressReport(int patientId)
        {
            return null;
        }

        public DataTable GetMostActivePatients(int topCount, DateTime startDate, DateTime endDate)
        {
            return null;
        }

        public DataTable GetDeviceUsageStatistics(DateTime startDate, DateTime endDate)
        {
            return null;
        }

        private string HashPassword(string password)
        {
            return null;
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return false;
        }

        private void LogError(Exception ex, string methodName)
        {
            // Hata loglama
        }

        public DataTable GetPatientSessionsByTc(string tc)
        {
            DataTable dt = new DataTable();

            string query = @"
        SELECT
            h.tc_kimlik_no AS ""TC Kimlik No"",
            h.ad || ' ' || h.soyad AS ""Hasta Adı"",
            s.seans_id AS ""Seans ID"",
            s.seans_tarihi_baslangic AS ""Seans Başlangıç"",
            s.seans_suresi_dk AS ""Süre (dk)"",
            s.yurume_hizi_km_s AS ""Yürüme Hızı"",
            s.vucut_agirlik_destegi_yuzde AS ""Ağırlık Desteği"",
            s.yurunen_mesafe_m AS ""Mesafe (m)"",
            s.zorluk_seviyesi AS ""Zorluk"",
            s.ortalama_kalp_atisi_bpm AS ""Ortalama Nabız"",
            s.notlar AS ""Notlar""
        FROM hastalar h
        INNER JOIN seanslar s 
            ON h.tc_kimlik_no = s.hasta_tc
        WHERE h.tc_kimlik_no = @tc
        ORDER BY s.seans_tarihi_baslangic DESC;
    ";

            try
            {
                using (var conn = new Npgsql.NpgsqlConnection(
                    "Host=localhost;Port=5432;Database=lokomatDB;Username=postgres;Password=1234;"))
                {
                    conn.Open();

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@tc", tc);

                        using (var da = new Npgsql.NpgsqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dt;
        }


    }
    // LoadCell verisi için model class
    public class LoadCellData
    {
        public DateTime Timestamp { get; set; }
        public double RightHeel { get; set; }
        public double LeftHeel { get; set; }
        public double RightToe { get; set; }
        public double LeftToe { get; set; }
        public double WeightBalance { get; set; }
        public int Index { get; set; }
    }
}
