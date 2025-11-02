using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LokomatUygulamasi.Modeller
{
    [Table("roller")]
    public class Rol
    {
        [Key]
        [Column("rol_id")]
        public int RolId { get; set; }
        [Required]
        [Column("rol_adi")]
        public string RolAdi { get; set; }
    }
}