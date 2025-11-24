using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LokomatUygulamasi.Modeller
{
    [Table("cinsiyetler")]
    public class Cinsiyet
    {
        [Key]
        [Column("cinsiyet_id")]
        public int CinsiyetId { get; set; }
        [Required]
        [Column("cinsiyet_adi")]
        public string CinsiyetAdi { get; set; }
    }
}