using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LokomatUygulamasi.Modeller
{
    [Table("yetkiler")]
    public class Yetki
    {
        [Key]
        [Column("yetki_id")]
        public int YetkiId { get; set; }
        [Required]
        [Column("yetki_kodu")]
        public string YetkiKodu { get; set; }
        [Column("aciklama")]
        public string Aciklama { get; set; }
    }
}