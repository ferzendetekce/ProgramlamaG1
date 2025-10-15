using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LokomatUygulamasi.Modeller
{
    [Table("loglar")]
    public class Log
    {
        [Key]
        [Column("log_id")]
        public long LogId { get; set; }
        [Required]
        [Column("log_seviyesi")]
        public string LogSeviyesi { get; set; }
        [Required]
        [Column("mesaj")]
        public string Mesaj { get; set; }
        [Column("olay_zamani")]
        public DateTime? OlayZamani { get; set; }
        [Column("hesap_id")]
        public int? HesapId { get; set; }
        [Column("ip_adresi")]
        public string IpAdresi { get; set; }
    }
}