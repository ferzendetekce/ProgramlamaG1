using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LokomatUygulamasi.Modeller
{
    [Table("personeltipleri")]
    public class PersonelTipi
    {
        [Key]
        [Column("personel_tip_id")]
        public int PersonelTipId { get; set; }
        [Required]
        [Column("tip_adi")]
        public string TipAdi { get; set; }
    }
}