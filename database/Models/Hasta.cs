using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokomatUygulamasi.Modeller
{
    [Table("hastalar")]
    public class Hasta
    {
        [Key]
        [Column("hasta_id")]
        public int HastaId { get; set; }

        [Required]
        [Column("tc_kimlik_no")]
        public string TCKimlikNo { get; set; }

        [Required]
        [Column("ad")]
        public string Ad { get; set; }

        [Required]
        [Column("soyad")]
        public string Soyad { get; set; }

        [Column("dogum_tarihi")]
        public DateTime? DogumTarihi { get; set; }

        [Column("cinsiyet_id")]
        [ForeignKey("Cinsiyet")]
        public int? CinsiyetId { get; set; }
        public Cinsiyet Cinsiyet { get; set; } 
        [Column("boy_cm")]
        public int? BoyCm { get; set; }

        [Column("kilo_kg")]
        public decimal? KiloKg { get; set; }

        [Column("bacak_boyu_cm")]
        public int? BacakBoyuCm { get; set; }

        [Column("kalca_diz_boyu_cm")]
        public int? KalcaDizBoyuCm { get; set; }

        [Column("diz_bilek_boyu_cm")]
        public int? DizBilekBoyuCm { get; set; }

        [Column("ayak_numarasi")]
        public int? AyakNumarasi { get; set; }

        [Column("teshis")]
        public string Teshis { get; set; }

        [Column("teshis_aciklamasi")]
        public string TeshisAciklamasi { get; set; }

        [Column("rahatsizlandigi_tarih")]
        public DateTime? RahatsizlandigiTarih { get; set; }

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
        [Column("olusturulma_tarihi")]
        public DateTime? OlusturulmaTarihi { get; set; }

        [Column("son_giris_tarihi")]
        public DateTime? SonGirisTarihi { get; set; }
    }
}