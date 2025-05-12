using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Lego_Set_Verwaltungssytem.Models;
using System.Security.Cryptography;


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
                 .HasKey(bs => bs.BenutzerSetId);

            modelBuilder.Entity<BenutzerSet>()
                .Property(bs => bs.BenutzerSetId)
                .ValueGeneratedOnAdd();  // <-- AutoIncrement aktivieren


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


        public void SeedTestdaten()
        {
            // Benutzer prüfen
            if (!Benutzer.Any(b => b.Benutzername == "Test"))
            {
                string pw = "Test";
                string hash = PasswortHelper.HashPasswort(pw);

                var testUser = new Benutzer
                {
                    Benutzername = "Test",
                    Email = "Test@gmail.com",
                    PasswortHash = hash,
                    Sicherheitscode = "123456" // Einfach für Test
                };

                Benutzer.Add(testUser);
                SaveChanges();

                // Sets definieren
                var sets = new List<LegoSet>
        {
            new LegoSet { Nummer = "10281", Name = "Bonsai Tree", Thema = "Botanical", Jahr = 2021, PreisUVP = 49.99 },
            new LegoSet { Nummer = "75304", Name = "Darth Vader Helm", Thema = "Star Wars", Jahr = 2021, PreisUVP = 69.99 },
            new LegoSet { Nummer = "75350", Name = "Clone Commander Cody", Thema = "Star Wars", Jahr = 2023, PreisUVP = 69.99 },
            new LegoSet { Nummer = "75375", Name = "Millennium Falcon", Thema = "Star Wars", Jahr = 2024, PreisUVP = 84.99 },
            new LegoSet { Nummer = "75328", Name = "Mando Helm", Thema = "Star Wars", Jahr = 2022, PreisUVP = 69.99 },
            new LegoSet { Nummer = "76191", Name = "Infinity Gauntlet", Thema = "Marvel", Jahr = 2021, PreisUVP = 79.99 },
            new LegoSet { Nummer = "75349", Name = "Captain Rex Helm", Thema = "Star Wars", Jahr = 2023, PreisUVP = 69.99 },
        };

                Sets.AddRange(sets);
                SaveChanges();

                // BenutzerSets mit gezahltem Preis
                var userId = testUser.BenutzerId;

                var benutzerSets = new List<BenutzerSet>
                {
                    new BenutzerSet { BenutzerId = userId, SetId = sets[0].SetId, Anzahl = 1, GezahlterPreis = 39.99, Kaufdatum = DateTime.Now, Notizen = "Test" },
                    new BenutzerSet { BenutzerId = userId, SetId = sets[1].SetId, Anzahl = 1, GezahlterPreis = 69.99, Kaufdatum = DateTime.Now, Notizen = "Test" },
                    new BenutzerSet { BenutzerId = userId, SetId = sets[2].SetId, Anzahl = 1, GezahlterPreis = 59.99, Kaufdatum = DateTime.Now, Notizen = "Test" },
                    new BenutzerSet { BenutzerId = userId, SetId = sets[3].SetId, Anzahl = 1, GezahlterPreis = 84.99, Kaufdatum = DateTime.Now, Notizen = "Test" },
                    new BenutzerSet { BenutzerId = userId, SetId = sets[4].SetId, Anzahl = 1, GezahlterPreis = 64.99, Kaufdatum = DateTime.Now, Notizen = "Test" },
                    new BenutzerSet { BenutzerId = userId, SetId = sets[5].SetId, Anzahl = 1, GezahlterPreis = 59.99, Kaufdatum = DateTime.Now, Notizen = "Test" },
                    new BenutzerSet { BenutzerId = userId, SetId = sets[6].SetId, Anzahl = 1, GezahlterPreis = 69.99, Kaufdatum = DateTime.Now, Notizen = "Test" },
                };

                BenutzerSets.AddRange(benutzerSets);
                SaveChanges();
            }
        }

    }

    public static class PasswortHelper
    {
        public static string HashPasswort(string passwort)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(passwort));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}

