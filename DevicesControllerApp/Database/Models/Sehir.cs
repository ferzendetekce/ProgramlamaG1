using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LokomatUygulamasi.Modeller
{
    [Table("sehirler")]
    public class Sehir
    {
        [Key]
        [Column("plaka_kodu")]
        public int PlakaKodu { get; set; }
        [Required]
        [Column("sehir_adi")]
        public string SehirAdi { get; set; }
    }
}