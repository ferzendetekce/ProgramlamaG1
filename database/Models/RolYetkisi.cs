using System.ComponentModel.DataAnnotations.Schema;
namespace LokomatUygulamasi.Modeller
{
    [Table("rolyetkileri")]
    public class RolYetkisi
    {
        [Column("rol_id")]
        public int RolId { get; set; }
        public Rol Rol { get; set; }
        [Column("yetki_id")]
        public int YetkiId { get; set; }
        public Yetki Yetki { get; set; }
    }
}