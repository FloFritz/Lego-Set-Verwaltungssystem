using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Lego_Set_Verwaltungssytem.Models;

namespace Lego_Set_Verwaltungssytem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Benutzer> Benutzer { get; set; }
        public DbSet<LegoSet> Sets { get; set; }
        public DbSet<BenutzerSet> BenutzerSets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=legosets.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LegoSet>()
                .HasKey(x => x.SetId); // <<<< Hier explizit sagen: SetId ist der Primärschlüssel

            modelBuilder.Entity<BenutzerSet>()
                .HasKey(bs => new { bs.BenutzerId, bs.SetId });

            modelBuilder.Entity<BenutzerSet>()
                .HasOne(bs => bs.Benutzer)
                .WithMany(b => b.BenutzerSets)
                .HasForeignKey(bs => bs.BenutzerId);

            modelBuilder.Entity<BenutzerSet>()
                .HasOne(bs => bs.Set)
                .WithMany(s => s.BenutzerSets)
                .HasForeignKey(bs => bs.SetId);
        }


        public void EnsureDatabase()
        {
            this.Database.EnsureCreated();
        }
    }
}

