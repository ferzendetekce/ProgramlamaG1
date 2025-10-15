using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokomatUygulamasi.Modeller
{
    [Table("seanslar")]
    public class Seans
    {
        [Key]
        [Column("seans_id")]
        public int SeansId { get; set; }

        [Column("hasta_id")]
        [ForeignKey("Hasta")]
        public int HastaId { get; set; }
        public Hasta Hasta { get; set; } // Navigation Property

        [Column("personel_id")]
        [ForeignKey("Personel")]
        public int PersonelId { get; set; }
        public Personel Personel { get; set; } // Navigation Property

        [Column("seans_tarihi_baslangic")]
        public DateTime? SeansTarihiBaslangic { get; set; }

        [Column("seans_suresi_dk")]
        public int? SeansSuresiDk { get; set; }

        [Column("yurume_hizi_km_s")]
        public decimal? YurumeHiziKmS { get; set; }

        [Column("vucut_agirlik_destegi_yuzde")]
        public int? VucutAgirlikDestegiYuzde { get; set; }

        [Column("yurunen_mesafe_m")]
        public int? YurunenMesafeM { get; set; }

        [Column("zorluk_seviyesi")]
        public string ZorlukSeviyesi { get; set; }

        [Column("ortalama_kalp_atisi_bpm")]
        public int? OrtalamaKalpAtisiBpm { get; set; }

        [Column("notlar")]
        public string Notlar { get; set; }
    }
}