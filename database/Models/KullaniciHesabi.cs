using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokomatUygulamasi.Modeller
{
    [Table("kullanicihesaplari")]
    public class KullaniciHesabi
    {
        [Key]
        [Column("hesap_id")]
        public int HesapId { get; set; }

        [Column("personel_id")]
        [ForeignKey("Personel")]
        public int? PersonelId { get; set; }
        public Personel Personel { get; set; } 

        [Column("rol_id")]
        [ForeignKey("Rol")]
        public int RolId { get; set; }
        public Rol Rol { get; set; }

        [Required]
        [Column("kullanici_adi")]
        public string KullaniciAdi { get; set; }

        [Required]
        [Column("sifre_hash")]
        public string SifreHash { get; set; }

        [Column("aktif_mi")]
        public bool? AktifMi { get; set; }

        [Column("olusturulma_tarihi")]
        public DateTime? OlusturulmaTarihi { get; set; }

        [Column("son_giris_tarihi")]
        public DateTime? SonGirisTarihi { get; set; }
    }
}