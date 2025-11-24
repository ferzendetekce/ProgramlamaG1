using System;
using Microsoft.EntityFrameworkCore;


namespace Lokomat.Modeller
{
    public class LokomatContext : DbContext
    {
        public DbSet<Cinsiyet> Cinsiyetler { get; set; }
        public DbSet<Hasta> Hastalar { get; set; }
        public DbSet<KullaniciHesabi> KullaniciHesaplari { get; set; }
        public DbSet<Log> Loglar { get; set; }
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<PersonelTipi> PersonelTipleri { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<RolYetkisi> RolYetkileri { get; set; }
        public DbSet<Seans> Seanslar { get; set; }
        public DbSet<Sehir> Sehirler { get; set; }
        public DbSet<Yetki> Yetkiler { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Host=localhost; Port=5432; Database=postgres; Username=postgres; Password=4205312";
            optionsBuilder.UseNpgsql(connectionString);
        }

 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolYetkisi>()
                .HasKey(ry => new { ry.RolId, ry.YetkiId });
        }
    }
}
