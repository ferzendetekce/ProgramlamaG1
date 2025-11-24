using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokomatUygulamasi.Modeller
{
    [Table("personeller")]
    public class Personel
    {
        [Key]
        [Column("personel_id")]
        public int PersonelId { get; set; }

        [Required]
        [Column("ad")]
        public string Ad { get; set; }

        [Required]
        [Column("soyad")]
        public string Soyad { get; set; }

        [Column("telefon")]
        public string Telefon { get; set; }

        [Column("e_posta")]
        public string EPosta { get; set; }

        [Column("adres")]
        public string Adres { get; set; }

        [Column("sehir_plaka_kodu")]
        [ForeignKey("Sehir")]
        public int? SehirPlakaKodu { get; set; }
        public Sehir Sehir { get; set; }

        [Column("personel_tip_id")]
        [ForeignKey("PersonelTipi")]
        public int PersonelTipId { get; set; }
        public PersonelTipi PersonelTipi { get; set; } 

        [Column("kayit_tarihi")]
        public DateTime? KayitTarihi { get; set; }
    }
}