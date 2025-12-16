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
    

        // Constructor'ı PUBLIC yaptık (Erişim hatası düzeldi)
        public DatabaseManager()
        {

        }

        

        public bool OpenConnection() { return false; }
        public bool CloseConnection() { return false; }
        // ... (Diğer boş metodlarınız buraya gelecek) ...

    }



    // --- MODELLER ---

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

    public class AppSettingModel
    {
        public string Theme { get; set; }
        public string Language { get; set; }
        public string DateFormat { get; set; }
        public string InstitutionName { get; set; }
    }
}